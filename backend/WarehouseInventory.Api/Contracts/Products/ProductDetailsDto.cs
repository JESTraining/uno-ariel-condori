namespace WarehouseInventory.Api.Contracts.Products;

public record ProductDetailsDto(
    Guid Id,
    string Sku,
    string? Barcode,
    string Name,
    string Category,
    string Location,
    decimal Price,
    int CurrentStock,
    int ReorderThreshold,
    bool IsActive,
    Guid Version
);
