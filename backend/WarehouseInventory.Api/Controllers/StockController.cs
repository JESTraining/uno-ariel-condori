using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseInventory.Api.Contracts.Products;
using WarehouseInventory.Api.Contracts.Stock;
using WarehouseInventory.Api.Infrastructure.Repositories;
using WarehouseInventory.Api.Mapping;
using WarehouseInventory.Api.Services;

namespace WarehouseInventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockController(IUnitOfWork unitOfWork, StockService stockService) : ControllerBase
{
    [HttpPost("movements")]
    public async Task<ActionResult<StockMovementResultDto>> SaveMovement(
        StockMovementRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await stockService.SaveMovementAsync(request, cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }
        catch (InvalidOperationException exception) when (exception.Message == "Insufficient stock")
        {
            return BadRequest(new { message = "Insufficient stock" });
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(new { message = "Stock was modified by another user. Refresh and retry." });
        }
    }

    [HttpGet("alert/low")]
    public async Task<ActionResult<IReadOnlyCollection<ProductListItemDto>>> GetLowStock(
        CancellationToken cancellationToken)
    {
        var products = await unitOfWork.Products.GetLowStockAsync(cancellationToken);
        return Ok(products.Select(product => product.ToListItemDto()).ToArray());
    }

    [HttpGet("movements/{productId:guid}")]
    public async Task<ActionResult<IReadOnlyCollection<StockMovementDto>>> GetMovementHistory(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var product = await unitOfWork.Products.GetByIdAsync(productId, cancellationToken);
        if (product is null)
        {
            return NotFound();
        }

        var movements = await unitOfWork.StockMovements.GetByProductIdAsync(productId, cancellationToken);
        return Ok(movements.Select(movement => movement.ToDto()).ToArray());
    }
}
