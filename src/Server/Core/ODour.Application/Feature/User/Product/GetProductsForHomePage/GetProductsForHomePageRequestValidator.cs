using FastEndpoints;
using FluentValidation;

namespace ODour.Application.Feature.User.Product.GetProductsForHomePage;

public sealed class GetProductsForHomePageRequestValidator
    : Validator<GetProductsForHomePageRequest>
{
    public GetProductsForHomePageRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;
    }
}
