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
            .Must(predicate: currentPage => currentPage > default(int));
    }
}
