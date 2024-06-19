using FastEndpoints;
using FluentValidation;

namespace ODour.Application.Feature.Admin.Order.SwitchOrderStatusToDeliveringSuccessfully;

public sealed class SwitchOrderStatusToDeliveringSuccessfullyRequestValidator
    : Validator<SwitchOrderStatusToDeliveringSuccessfullyRequest>
{
    public SwitchOrderStatusToDeliveringSuccessfullyRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(expression: request => request.OrderId).NotEmpty();
    }
}
