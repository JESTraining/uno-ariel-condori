using WarehouseInventory.Api.Contracts.Catalogs;

namespace WarehouseInventory.Api.Services;

public interface ICatalogService
{
    Task<IReadOnlyCollection<CategoryDto>> GetAllCategories(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<LocationDto>> GetAllLocations(CancellationToken cancellationToken);
}
