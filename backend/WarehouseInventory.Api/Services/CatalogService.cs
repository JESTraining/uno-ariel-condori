using Microsoft.EntityFrameworkCore;
using WarehouseInventory.Api.Contracts.Catalogs;
using WarehouseInventory.Api.Domain.Entities;
using WarehouseInventory.Api.Infrastructure.Data;
using WarehouseInventory.Api.Infrastructure.Repositories;

namespace WarehouseInventory.Api.Services;

public class CatalogService(WarehouseDbContext dbContext, ILogger<CatalogService> logger) : ICatalogService
{
    public async Task<IReadOnlyCollection<CategoryDto>> GetAllCategories(CancellationToken cancellationToken)
    {
        logger.LogInformation("Listing all categories");

        var categories = await dbContext.Categories.AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

        return categories.Select(c => new CategoryDto(c.Id, c.Name)).ToArray();
    }

    public async Task<IReadOnlyCollection<LocationDto>> GetAllLocations(CancellationToken cancellationToken)
    {
        logger.LogInformation("Listing all locations");

        var locations = await dbContext.WarehouseLocations.AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

        return locations.Select(c => new LocationDto(c.Id, c.Name)).ToArray();
    }
}
