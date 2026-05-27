using WarehouseInventory.Api.Contracts.Products;

namespace WarehouseInventory.Api.Services;

public interface IProductService
{
    Task<PagedResult<ProductListItemDto>> GetProducts(ProductQuery query, CancellationToken cancellationToken);

    Task<ProductDetailsDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<ProductDetailsDto?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken);

    Task<ProductDetailsDto> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken);

    Task<ProductDetailsDto> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
