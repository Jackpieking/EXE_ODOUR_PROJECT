using FastEndpoints;
using FluentValidation;

namespace ODour.Application.Feature.User.Cart.RemoveFromCart;

public sealed class RemoveFromCartRequestValidator : Validator<RemoveFromCartRequest>
{
    public RemoveFromCartRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(expression: request => request.ProductId).NotEmpty();

        RuleFor(expression: request => request.Quantity)
            .NotEmpty()
            .GreaterThan(valueToCompare: default);
    }
}
