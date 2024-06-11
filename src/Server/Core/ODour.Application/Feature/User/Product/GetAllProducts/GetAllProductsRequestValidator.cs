using FastEndpoints;
using FluentValidation;

namespace ODour.Application.Feature.User.Product.GetAllProducts;

public sealed class GetAllProductsRequestValidator : Validator<GetAllProductsRequest>
{
    public GetAllProductsRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(expression: request => request.CurrentPage)
            .NotEmpty()
            .GreaterThan(valueToCompare: default);

        RuleFor(expression: request => request.SortType).NotEmpty();

        RuleFor(expression: request => request.CategoryId).NotEmpty();
    }
}
