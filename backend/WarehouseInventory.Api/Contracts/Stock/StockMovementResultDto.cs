namespace WarehouseInventory.Api.Contracts.Stock;

public record StockMovementResultDto(
    Guid ProductId,
    int PreviousStock,
    int NewStock,
    StockMovementDto Movement,
    Guid Version
);
