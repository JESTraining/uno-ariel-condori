using Microsoft.EntityFrameworkCore;
using WarehouseInventory.Api.Contracts.Stock;
using WarehouseInventory.Api.Domain.Entities;
using WarehouseInventory.Api.Infrastructure.Data;
using WarehouseInventory.Api.Infrastructure.Repositories;
using WarehouseInventory.Api.Mapping;

namespace WarehouseInventory.Api.Services;

public class StockService(
    IUnitOfWork unitOfWork,
    WarehouseDbContext dbContext,
    ILogger<StockService> logger)
{
    public async Task<StockMovementResultDto?> RecordMovementAsync(
        StockMovementRequest request,
        CancellationToken cancellationToken)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        var product = await unitOfWork.Products.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
        {
            return null;
        }

        dbContext.Entry(product).Property(entity => entity.Version).OriginalValue = request.Version;

        var previousStock = product.CurrentStock;
        var newStock = previousStock + request.QuantityChange;
        if (newStock < 0)
        {
            throw new InvalidOperationException("Insufficient stock");
        }

        product.CurrentStock = newStock;
        product.Version = Guid.NewGuid();
        var movement = new StockMovement
        {
            ProductId = product.Id,
            QuantityChange = request.QuantityChange,
            PreviousStock = previousStock,
            NewStock = newStock,
            Reason = request.Reason.Trim(),
            CreatedBy = string.IsNullOrWhiteSpace(request.CreatedBy) ? "warehouse1" : request.CreatedBy.Trim()
        };

        await unitOfWork.StockMovements.AddAsync(movement, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        logger.LogInformation(
            "Stock changed for product {ProductId}. QuantityChange={QuantityChange}, PreviousStock={PreviousStock}, NewStock={NewStock}, Reason={Reason}, CreatedBy={CreatedBy}, CreatedAt={CreatedAt}",
            product.Id,
            movement.QuantityChange,
            movement.PreviousStock,
            movement.NewStock,
            movement.Reason,
            movement.CreatedBy,
            movement.CreatedAt);

        return new StockMovementResultDto(product.Id, previousStock, newStock, movement.ToDto());
    }
}
