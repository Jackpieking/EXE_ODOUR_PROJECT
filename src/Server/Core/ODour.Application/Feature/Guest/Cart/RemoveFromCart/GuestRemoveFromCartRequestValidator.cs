using FastEndpoints;
using FluentValidation;

namespace ODour.Application.Feature.Guest.Cart.RemoveFromCart;

public sealed class GuestRemoveFromCartRequestValidator : Validator<GuestRemoveFromCartRequest>
{
    public GuestRemoveFromCartRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(expression: request => request.ProductId).NotEmpty();

        RuleFor(expression: request => request.Quantity)
            .NotEmpty()
            .GreaterThan(valueToCompare: default);
    }
}
