using FluentValidation;
using WarehouseInventory.Api.Contracts.Products;

namespace WarehouseInventory.Api.Validation;

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(request => request.Sku).NotEmpty().MaximumLength(64);
        RuleFor(request => request.Barcode).MaximumLength(128);
        RuleFor(request => request.Name).NotEmpty().MaximumLength(200);
        RuleFor(request => request.Category).NotEmpty().MaximumLength(120);
        RuleFor(request => request.Location).NotEmpty().MaximumLength(80);
        RuleFor(request => request.Price).GreaterThanOrEqualTo(0);
        RuleFor(request => request.ReorderThreshold).GreaterThanOrEqualTo(0);
        RuleFor(request => request.Version).NotEmpty();
    }
}

