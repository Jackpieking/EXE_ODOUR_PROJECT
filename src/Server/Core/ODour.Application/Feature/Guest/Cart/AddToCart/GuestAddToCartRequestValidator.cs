using FastEndpoints;
using FluentValidation;

namespace ODour.Application.Feature.Guest.Cart.AddToCart;

public sealed class GuestAddToCartRequestValidator : Validator<GuestAddToCartRequest>
{
    public GuestAddToCartRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(expression: request => request.ProductId).NotEmpty();

        RuleFor(expression: request => request.Quantity).NotEmpty().GreaterThan(valueToCompare: 0);
    }
}
