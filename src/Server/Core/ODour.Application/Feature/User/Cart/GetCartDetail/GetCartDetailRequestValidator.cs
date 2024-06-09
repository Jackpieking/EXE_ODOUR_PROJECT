using FastEndpoints;
using FluentValidation;

namespace ODour.Application.Feature.User.Cart.GetCartDetail;

public sealed class GetCartDetailRequestValidator : Validator<GetCartDetailRequest>
{
    public GetCartDetailRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(expression: request => request.GetUserId()).NotEmpty();
    }
}
