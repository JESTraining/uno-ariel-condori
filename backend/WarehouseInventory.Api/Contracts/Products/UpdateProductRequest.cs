namespace WarehouseInventory.Api.Contracts.Products;

public record UpdateProductRequest(
    string Sku,
    string? Barcode,
    string Name,
    string Category,
    string Location,
    decimal Price,
    int ReorderThreshold,
    Guid Version
);
