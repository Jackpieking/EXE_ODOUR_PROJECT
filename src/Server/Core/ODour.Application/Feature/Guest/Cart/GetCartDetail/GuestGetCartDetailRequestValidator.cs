using FastEndpoints;
using FluentValidation;

namespace ODour.Application.Feature.Guest.Cart.GetCartDetail;

public sealed class GuestGetCartDetailRequestValidator : Validator<GuestGetCartDetailRequest>
{
    public GuestGetCartDetailRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;
    }
}
