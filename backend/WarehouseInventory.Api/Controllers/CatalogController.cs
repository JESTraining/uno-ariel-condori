using Microsoft.AspNetCore.Mvc;
using WarehouseInventory.Api.Contracts.Catalogs;
using WarehouseInventory.Api.Services;

namespace WarehouseInventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogController(ICatalogService categoryService) : ControllerBase
{
    [HttpGet("categories")]
    public async Task<ActionResult<IReadOnlyCollection<CategoryDto>>> List(CancellationToken cancellationToken)
    {
        var categories = await categoryService.GetAllCategories(cancellationToken);
        return Ok(categories);
    }

    [HttpGet("locations")]
    public async Task<ActionResult<IReadOnlyCollection<LocationDto>>> ListLocations(CancellationToken cancellationToken)
    {
        var locations = await categoryService.GetAllLocations(cancellationToken);
        return Ok(locations);
    }
}
