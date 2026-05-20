namespace WarehouseInventory.Api.Contracts.Products;

public record ProductListItemDto(
    Guid Id,
    string Sku,
    string? Barcode,
    string Name,
    string Category,
    string Location,
    decimal Price,
    int CurrentStock,
    int ReorderThreshold,
    bool IsLowStock);

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
    Guid Version);

public record ProductQuery(string? Search, string? Category, string? Location, int Page = 1, int PageSize = 20);

public record PagedResult<T>(IReadOnlyCollection<T> Items, int Page, int PageSize, int TotalCount);

public record CreateProductRequest(
    string Sku,
    string? Barcode,
    string Name,
    string Category,
    string Location,
    decimal Price,
    int ReorderThreshold);

public record UpdateProductRequest(
    string Sku,
    string? Barcode,
    string Name,
    string Category,
    string Location,
    decimal Price,
    int ReorderThreshold,
    Guid Version);
