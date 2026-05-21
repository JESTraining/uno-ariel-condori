namespace WarehouseInventory.Api.Contracts.Products;

public record CreateProductRequest(
    string Sku,
    string? Barcode,
    string Name,
    string Category,
    string Location,
    decimal Price,
    int ReorderThreshold
);
