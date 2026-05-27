namespace WarehouseInventory.Api.Contracts.Products;

public record ProductQuery(
    string? Search, 
    string? Category, 
    string? Location, 
    int Page = 1, 
    int PageSize = 20
);
