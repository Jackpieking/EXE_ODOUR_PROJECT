using FastEndpoints;
using FluentValidation;

namespace ODour.Application.Feature.User.Product.GetAllProducts;

public sealed class GetAllProductsRequestValidator : Validator<GetAllProductsRequest>
{
    public GetAllProductsRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;
    }
}
