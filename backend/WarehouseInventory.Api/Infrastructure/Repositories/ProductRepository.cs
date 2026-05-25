using Microsoft.EntityFrameworkCore;
using WarehouseInventory.Api.Contracts.Products;
using WarehouseInventory.Api.Domain.Entities;
using WarehouseInventory.Api.Infrastructure.Data;

namespace WarehouseInventory.Api.Infrastructure.Repositories;

public class ProductRepository(WarehouseDbContext dbContext) : IProductRepository
{
    public async Task<PagedResult<Product>> ListAsync(ProductQuery query, CancellationToken cancellationToken)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var products = dbContext.Products.AsNoTracking()
            .Include(product => product.Category)
            .Include(product => product.Location)
            .Where(product => product.IsActive);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            products = products.Where(product =>
                product.Name.ToLower().Contains(search) ||
                product.Sku.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            products = products.Where(product => product.Category != null && product.Category.Id == query.Category);
        }

        if (!string.IsNullOrWhiteSpace(query.Location))
        {
            products = products.Where(product => product.Location != null && product.Location.Id == query.Location);
        }

        var totalCount = await products.CountAsync(cancellationToken);
        var items = await products
            .OrderBy(product => product.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Product>(items, page, pageSize, totalCount);
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Products
            .Include(product => product.Category)
            .Include(product => product.Location)
            .FirstOrDefaultAsync(product => product.Id == id && product.IsActive, cancellationToken);

    public Task<Product?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken) =>
        dbContext.Products.AsNoTracking()
            .Include(product => product.Category)
            .Include(product => product.Location)
            .FirstOrDefaultAsync(product => product.Barcode == barcode && product.IsActive, cancellationToken);

    public async Task<IReadOnlyCollection<Product>> GetLowStockAsync(CancellationToken cancellationToken) =>
        await dbContext.Products.AsNoTracking()
            .Include(product => product.Category)
            .Include(product => product.Location)
            .Where(product => product.IsActive && product.CurrentStock < product.ReorderThreshold)
            .OrderBy(product => product.CurrentStock)
            .ThenBy(product => product.Name)
            .ToListAsync(cancellationToken);

    public Task AddAsync(Product product, CancellationToken cancellationToken) =>
        dbContext.Products.AddAsync(product, cancellationToken).AsTask();
}
