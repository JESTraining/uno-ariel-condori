using Microsoft.EntityFrameworkCore;
using WarehouseInventory.Api.Domain.Entities;
using WarehouseInventory.Api.Infrastructure.Data;

namespace WarehouseInventory.Api.Infrastructure.Repositories;

public class StockMovementRepository(WarehouseDbContext dbContext) : IStockMovementRepository
{
    public async Task<IReadOnlyCollection<StockMovement>> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken) =>
        await dbContext.StockMovements.AsNoTracking()
            .Where(movement => movement.ProductId == productId)
            .OrderByDescending(movement => movement.CreatedAt)
            .ToListAsync(cancellationToken);

    public Task AddAsync(StockMovement movement, CancellationToken cancellationToken) =>
        dbContext.StockMovements.AddAsync(movement, cancellationToken).AsTask();
}
