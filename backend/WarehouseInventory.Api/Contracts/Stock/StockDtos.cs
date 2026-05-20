namespace WarehouseInventory.Api.Contracts.Stock;

public record StockMovementRequest(
    Guid ProductId,
    int QuantityChange,
    string Reason,
    string? CreatedBy,
    Guid Version);

public record StockMovementDto(
    Guid Id,
    Guid ProductId,
    int QuantityChange,
    int PreviousStock,
    int NewStock,
    string Reason,
    DateTimeOffset CreatedAt,
    string CreatedBy);

public record StockMovementResultDto(
    Guid ProductId,
    int PreviousStock,
    int NewStock,
    StockMovementDto Movement);
