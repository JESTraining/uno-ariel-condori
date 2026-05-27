using WarehouseInventory.Api.Domain.Entities;

namespace WarehouseInventory.Api.Infrastructure.Repositories;

public interface IStockMovementRepository
{
    Task<IReadOnlyCollection<StockMovement>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken);
    Task AddAsync(StockMovement movement, CancellationToken cancellationToken);
}
