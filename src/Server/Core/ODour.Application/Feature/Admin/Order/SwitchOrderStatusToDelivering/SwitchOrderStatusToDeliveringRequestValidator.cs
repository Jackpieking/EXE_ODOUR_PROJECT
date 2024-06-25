using FastEndpoints;
using FluentValidation;

namespace ODour.Application.Feature.Admin.Order.SwitchOrderStatusToDelivering;

public sealed class SwitchOrderStatusToDeliveringRequestValidator
    : Validator<SwitchOrderStatusToDeliveringRequest>
{
    public SwitchOrderStatusToDeliveringRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(expression: request => request.OrderId).NotEmpty();
    }
}
