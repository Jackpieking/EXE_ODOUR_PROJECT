using FastEndpoints;
using FluentValidation;

namespace ODour.Application.Feature.User.Cart.AddToCart;

public sealed class AddToCartRequestValidator : Validator<AddToCartRequest>
{
    public AddToCartRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(expression: request => request.ProductId).NotEmpty();

        RuleFor(expression: request => request.Quantity)
            .NotEmpty()
            .GreaterThan(valueToCompare: default);
    }
}
