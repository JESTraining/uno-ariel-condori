namespace WarehouseInventory.Api.Contracts.Stock;

public record StockMovementRequest(
    Guid ProductId,
    int QuantityChange,
    string Reason,
    string? CreatedBy,
    Guid Version
);