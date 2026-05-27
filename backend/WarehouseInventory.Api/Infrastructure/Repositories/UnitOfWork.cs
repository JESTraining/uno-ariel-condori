using Microsoft.EntityFrameworkCore.Storage;
using WarehouseInventory.Api.Infrastructure.Data;

namespace WarehouseInventory.Api.Infrastructure.Repositories;

public class UnitOfWork(WarehouseDbContext dbContext) : IUnitOfWork
{
    public IProductRepository Products { get; } = new ProductRepository(dbContext);
    public IStockMovementRepository StockMovements { get; } = new StockMovementRepository(dbContext);

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken) =>
        dbContext.Database.BeginTransactionAsync(cancellationToken);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
