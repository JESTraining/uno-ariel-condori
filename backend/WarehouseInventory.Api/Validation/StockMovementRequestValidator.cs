using FluentValidation;
using WarehouseInventory.Api.Contracts.Stock;

namespace WarehouseInventory.Api.Validation;

public class StockMovementRequestValidator : AbstractValidator<StockMovementRequest>
{
    public StockMovementRequestValidator()
    {
        RuleFor(request => request.ProductId).NotEmpty();
        RuleFor(request => request.QuantityChange).NotEqual(0);
        RuleFor(request => request.Reason).NotEmpty().MaximumLength(500);
        RuleFor(request => request.CreatedBy).MaximumLength(120);
        RuleFor(request => request.Version).NotEmpty();
    }
}
