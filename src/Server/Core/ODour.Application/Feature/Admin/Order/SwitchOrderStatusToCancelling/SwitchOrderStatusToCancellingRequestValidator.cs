using FastEndpoints;
using FluentValidation;

namespace ODour.Application.Feature.Admin.Order.SwitchOrderStatusToCancelling;

public sealed class SwitchOrderStatusToCancellingRequestValidator
    : Validator<SwitchOrderStatusToCancellingRequest>
{
    public SwitchOrderStatusToCancellingRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(expression: request => request.OrderId).NotEmpty();
    }
}
