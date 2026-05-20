using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using WarehouseInventory.Api.Contracts.Products;
using WarehouseInventory.Api.Domain.Entities;
using WarehouseInventory.Api.Infrastructure.Data;
using WarehouseInventory.Api.Infrastructure.Repositories;
using WarehouseInventory.Api.Mapping;

namespace WarehouseInventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IUnitOfWork unitOfWork, WarehouseDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductListItemDto>>> List(
        [FromQuery] ProductQuery query,
        CancellationToken cancellationToken)
    {
        var result = await unitOfWork.Products.ListAsync(query, cancellationToken);
        return Ok(new PagedResult<ProductListItemDto>(
            result.Items.Select(product => product.ToListItemDto()).ToArray(),
            result.Page,
            result.PageSize,
            result.TotalCount));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDetailsDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var product = await unitOfWork.Products.GetByIdAsync(id, cancellationToken);
        return product is null ? NotFound() : Ok(product.ToDetailsDto());
    }

    [HttpGet("barcode/{barcode}")]
    public async Task<ActionResult<ProductDetailsDto>> GetByBarcode(string barcode, CancellationToken cancellationToken)
    {
        var product = await unitOfWork.Products.GetByBarcodeAsync(barcode, cancellationToken);
        return product is null ? NotFound() : Ok(product.ToDetailsDto());
    }

    [HttpPost]
    public async Task<ActionResult<ProductDetailsDto>> Create(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var category = await GetOrCreateCategoryAsync(request.Category, cancellationToken);
        var location = await GetOrCreateLocationAsync(request.Location, cancellationToken);
        var product = new Product
        {
            Sku = request.Sku.Trim(),
            Barcode = string.IsNullOrWhiteSpace(request.Barcode) ? null : request.Barcode.Trim(),
            Name = request.Name.Trim(),
            Category = category,
            Location = location,
            Price = request.Price,
            ReorderThreshold = request.ReorderThreshold
        };

        await unitOfWork.Products.AddAsync(product, cancellationToken);

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            return Conflict(new { message = "SKU or barcode already exists." });
        }

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product.ToDetailsDto());
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductDetailsDto>> Update(
        Guid id,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = await unitOfWork.Products.GetByIdAsync(id, cancellationToken);
        if (product is null)
        {
            return NotFound();
        }

        dbContext.Entry(product).Property(entity => entity.Version).OriginalValue = request.Version;
        product.Sku = request.Sku.Trim();
        product.Barcode = string.IsNullOrWhiteSpace(request.Barcode) ? null : request.Barcode.Trim();
        product.Name = request.Name.Trim();
        product.Category = await GetOrCreateCategoryAsync(request.Category, cancellationToken);
        product.Location = await GetOrCreateLocationAsync(request.Location, cancellationToken);
        product.Price = request.Price;
        product.ReorderThreshold = request.ReorderThreshold;
        product.Version = Guid.NewGuid();

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(new { message = "Product was modified by another user. Refresh and retry." });
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            return Conflict(new { message = "SKU or barcode already exists." });
        }

        return Ok(product.ToDetailsDto());
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var product = await unitOfWork.Products.GetByIdAsync(id, cancellationToken);
        if (product is null)
        {
            return NotFound();
        }

        product.IsActive = false;
        product.Version = Guid.NewGuid();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static bool IsUniqueViolation(DbUpdateException exception) =>
        exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };

    private async Task<Category> GetOrCreateCategoryAsync(string name, CancellationToken cancellationToken)
    {
        var normalizedName = name.Trim();
        var category = await dbContext.Categories
            .FirstOrDefaultAsync(item => item.Name.ToLower() == normalizedName.ToLower(), cancellationToken);

        if (category is not null)
        {
            return category;
        }

        category = new Category { Name = normalizedName };
        dbContext.Categories.Add(category);
        return category;
    }

    private async Task<WarehouseLocation> GetOrCreateLocationAsync(string code, CancellationToken cancellationToken)
    {
        var normalizedCode = code.Trim();
        var location = await dbContext.WarehouseLocations
            .FirstOrDefaultAsync(item => item.Code.ToLower() == normalizedCode.ToLower(), cancellationToken);

        if (location is not null)
        {
            return location;
        }

        location = new WarehouseLocation { Code = normalizedCode };
        dbContext.WarehouseLocations.Add(location);
        return location;
    }
}
