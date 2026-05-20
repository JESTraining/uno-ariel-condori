using WarehouseInventory.Api.Contracts.Products;
using WarehouseInventory.Api.Contracts.Stock;
using WarehouseInventory.Api.Domain.Entities;

namespace WarehouseInventory.Api.Mapping;

public static class ProductMapping
{
    public static ProductListItemDto ToListItemDto(this Product product) =>
        new(
            product.Id,
            product.Sku,
            product.Barcode,
            product.Name,
            product.Category?.Name ?? string.Empty,
            product.Location?.Code ?? string.Empty,
            product.Price,
            product.CurrentStock,
            product.ReorderThreshold,
            product.CurrentStock < product.ReorderThreshold);

    public static ProductDetailsDto ToDetailsDto(this Product product) =>
        new(
            product.Id,
            product.Sku,
            product.Barcode,
            product.Name,
            product.Category?.Name ?? string.Empty,
            product.Location?.Code ?? string.Empty,
            product.Price,
            product.CurrentStock,
            product.ReorderThreshold,
            product.IsActive,
            product.Version);

    public static StockMovementDto ToDto(this StockMovement movement) =>
        new(
            movement.Id,
            movement.ProductId,
            movement.QuantityChange,
            movement.PreviousStock,
            movement.NewStock,
            movement.Reason,
            movement.CreatedAt,
            movement.CreatedBy);
}
