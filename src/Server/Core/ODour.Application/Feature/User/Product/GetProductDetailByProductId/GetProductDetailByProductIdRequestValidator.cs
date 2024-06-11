using FastEndpoints;
using FluentValidation;

namespace ODour.Application.Feature.User.Product.GetProductDetailByProductId;

public sealed class GetProductDetailByProductIdRequestValidator
    : Validator<GetProductDetailByProductIdRequest>
{
    public GetProductDetailByProductIdRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(expression: request => request.ProductId).NotEmpty();
    }
}
