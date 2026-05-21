namespace WarehouseInventory.Api.Contracts.Stock;

public record StockMovementDto(
    Guid Id,
    Guid ProductId,
    int QuantityChange,
    int PreviousStock,
    int NewStock,
    string Reason,
    DateTimeOffset CreatedAt,
    string CreatedBy
);