using WarehouseInventory.Api.Contracts.Products;
using WarehouseInventory.Api.Domain.Entities;

namespace WarehouseInventory.Api.Infrastructure.Repositories;

public interface IProductRepository
{
    Task<PagedResult<Product>> ListAsync(ProductQuery query, CancellationToken cancellationToken);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Product?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Product>> GetLowStockAsync(CancellationToken cancellationToken);
    Task AddAsync(Product product, CancellationToken cancellationToken);
}
