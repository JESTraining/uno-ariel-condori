using Microsoft.EntityFrameworkCore.Storage;

namespace WarehouseInventory.Api.Infrastructure.Repositories;

public interface IUnitOfWork
{
    IProductRepository Products { get; }
    IStockMovementRepository StockMovements { get; }
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
