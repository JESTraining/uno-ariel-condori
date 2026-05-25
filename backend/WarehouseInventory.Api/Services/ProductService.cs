using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using AutoMapper;
using WarehouseInventory.Api.Contracts.Products;
using WarehouseInventory.Api.Domain.Entities;
using WarehouseInventory.Api.Infrastructure.Data;
using WarehouseInventory.Api.Infrastructure.Repositories;

namespace WarehouseInventory.Api.Services;

public class ProductService(
    IUnitOfWork unitOfWork,
    WarehouseDbContext dbContext,
    IMapper mapper,
    ILogger<ProductService> logger) : IProductService
{
    public async Task<PagedResult<ProductListItemDto>> GetProducts(ProductQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Listing products with query: Page={Page}, PageSize={PageSize}", query.Page, query.PageSize);
        
        var result = await unitOfWork.Products.ListAsync(query, cancellationToken);
        return new PagedResult<ProductListItemDto>(
            result.Items.Select(product => mapper.Map<ProductListItemDto>(product)).ToArray(),
            result.Page,
            result.PageSize,
            result.TotalCount);
    }

    public async Task<ProductDetailsDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting product by ID: {ProductId}", id);
        
        var product = await unitOfWork.Products.GetByIdAsync(id, cancellationToken);
        return product is null ? null : mapper.Map<ProductDetailsDto>(product);
    }

    public async Task<ProductDetailsDto?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting product by barcode: {Barcode}", barcode);
        
        var product = await unitOfWork.Products.GetByBarcodeAsync(barcode, cancellationToken);
        return product is null ? null : mapper.Map<ProductDetailsDto>(product);
    }

    public async Task<ProductDetailsDto> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating product with SKU: {SKU}", request.Sku);
        
        var category = await GetOrCreateCategoryAsync(request.Category, cancellationToken);
        var location = await GetOrCreateLocationAsync(request.Location, cancellationToken);
        
        var product = new Product
        {
            Sku = request.Sku.Trim(),
            Barcode = string.IsNullOrWhiteSpace(request.Barcode) ? null : request.Barcode.Trim(),
            Name = request.Name.Trim(),
            CategoryId = category.Id,
            LocationId = location.Id,
            Price = request.Price,
            ReorderThreshold = request.ReorderThreshold
        };

        await unitOfWork.Products.AddAsync(product, cancellationToken);

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Product created successfully with ID: {ProductId}", product.Id);
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            var field = GetUniqueViolationField(exception) ?? "SKU or barcode";
            logger.LogWarning("Product creation failed due to unique constraint violation on {Field}. SKU: {SKU}, Barcode: {Barcode}", 
                field, request.Sku, request.Barcode);
            throw new InvalidOperationException($"{field} already exists.", exception);
        }

        return mapper.Map<ProductDetailsDto>(product);
    }

    public async Task<ProductDetailsDto> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating product with ID: {ProductId}", id);
        
        var product = await unitOfWork.Products.GetByIdAsync(id, cancellationToken);
        if (product is null)
        {
            logger.LogWarning("Product not found for update. ID: {ProductId}", id);
            throw new InvalidOperationException("Product not found.");
        }

        dbContext.Entry(product).Property(entity => entity.Version).OriginalValue = request.Version;
        
        product.Sku = request.Sku.Trim();
        product.Barcode = string.IsNullOrWhiteSpace(request.Barcode) ? null : request.Barcode.Trim();
        product.Name = request.Name.Trim();
        
        var category = await GetOrCreateCategoryAsync(request.Category, cancellationToken);
        var location = await GetOrCreateLocationAsync(request.Location, cancellationToken);
        
        product.CategoryId = category.Id;
        product.LocationId = location.Id;
        product.Category = category;
        product.Location = location;
        product.Price = request.Price;
        product.ReorderThreshold = request.ReorderThreshold;
        product.Version = Guid.NewGuid();

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Product updated successfully with ID: {ProductId}", id);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            logger.LogWarning(exception, "Product update failed due to concurrency conflict. ID: {ProductId}", id);
            throw new InvalidOperationException("Product was modified by another user. Refresh and retry.", exception);
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            var field = GetUniqueViolationField(exception) ?? "SKU or barcode";
            logger.LogWarning("Product update failed due to unique constraint violation on {Field}. ID: {ProductId}, SKU: {SKU}", field, id, request.Sku);
            throw new InvalidOperationException($"{field} already exists.", exception);
        }

        return mapper.Map<ProductDetailsDto>(product);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting product with ID: {ProductId}", id);
        
        var product = await unitOfWork.Products.GetByIdAsync(id, cancellationToken);
        if (product is null)
        {
            logger.LogWarning("Product not found for deletion. ID: {ProductId}", id);
            throw new InvalidOperationException("Product not found.");
        }

        product.IsActive = false;
        product.Version = Guid.NewGuid();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Product soft-deleted successfully with ID: {ProductId}", id);
    }

    private static bool IsUniqueViolation(DbUpdateException exception) =>
        exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };

    private static string? GetUniqueViolationField(DbUpdateException exception)
    {
        if (exception.InnerException is not PostgresException pe || pe.SqlState != PostgresErrorCodes.UniqueViolation)
            return null;

        var detail = pe.Detail ?? string.Empty;
        if (detail.Contains("(sku)", StringComparison.OrdinalIgnoreCase) || detail.Contains("key (sku)", StringComparison.OrdinalIgnoreCase))
            return "SKU";
        if (detail.Contains("(barcode)", StringComparison.OrdinalIgnoreCase) || detail.Contains("key (barcode)", StringComparison.OrdinalIgnoreCase))
            return "Barcode";

        var constraint = pe.ConstraintName ?? string.Empty;
        if (constraint.IndexOf("sku", StringComparison.OrdinalIgnoreCase) >= 0)
            return "SKU";
        if (constraint.IndexOf("barcode", StringComparison.OrdinalIgnoreCase) >= 0)
            return "Barcode";

        return null;
    }

    private async Task<Category> GetOrCreateCategoryAsync(string id, CancellationToken cancellationToken)
    {
        var category = await dbContext.Categories
            .FirstOrDefaultAsync(item => item.Id.ToLower() == id.ToLower(), cancellationToken);

        if (category is not null)
        {
            logger.LogDebug("Found existing category: {CategoryId}", category.Id);
            return category;
        }

        category = new Category { Id = id, Name = id };
        dbContext.Categories.Add(category);
        logger.LogDebug("Created new category: {CategoryId} - {CategoryName}", id, id);
        return category;
    }

    private async Task<WarehouseLocation> GetOrCreateLocationAsync(string id, CancellationToken cancellationToken)
    {
        var location = await dbContext.WarehouseLocations
            .FirstOrDefaultAsync(item => item.Id.ToLower() == id.ToLower(), cancellationToken);

        if (location is not null)
        {
            logger.LogDebug("Found existing location: {LocationId}", location.Id);
            return location;
        }

        location = new WarehouseLocation { Id = id, Name = id };
        dbContext.WarehouseLocations.Add(location);
        logger.LogDebug("Created new location: {LocationId} - {LocationName}", id, id);
        return location;
    }

    private static string CreateEntityId(string name)
    {
        var id = name.Trim().ToUpperInvariant().Replace(" ", string.Empty);
        return id.Length <= 16 ? id : id[..16];
    }
}
