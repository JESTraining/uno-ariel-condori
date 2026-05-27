using WarehouseInventory.Api.Contracts.Stock;

namespace WarehouseInventory.Api.Services;

public interface IStockService
{
    Task<StockMovementResultDto?> SaveMovementAsync(
        StockMovementRequest request,
        CancellationToken cancellationToken);
}
