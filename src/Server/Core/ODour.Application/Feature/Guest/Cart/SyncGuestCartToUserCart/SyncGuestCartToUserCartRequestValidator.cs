using FastEndpoints;
using FluentValidation;

namespace ODour.Application.Feature.Guest.Cart.SyncGuestCartToUserCart;

public sealed class SyncGuestCartToUserCartRequestValidator
    : Validator<SyncGuestCartToUserCartRequest>
{
    public SyncGuestCartToUserCartRequestValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;
    }
}
