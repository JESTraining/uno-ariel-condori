using FluentValidation;
using WarehouseInventory.Api.Contracts.Products;

namespace WarehouseInventory.Api.Validation;

public class ProductQueryValidator : AbstractValidator<ProductQuery>
{
    public ProductQueryValidator()
    {
        RuleFor(request => request.Page).GreaterThan(0);
        RuleFor(request => request.PageSize).InclusiveBetween(1, 100);
    }
}

