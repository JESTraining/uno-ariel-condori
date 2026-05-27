namespace WarehouseInventory.Api.Contracts.Products;

public record PagedResult<T>(
    IReadOnlyCollection<T> Items, 
    int Page, 
    int PageSize, 
    int TotalCount
);