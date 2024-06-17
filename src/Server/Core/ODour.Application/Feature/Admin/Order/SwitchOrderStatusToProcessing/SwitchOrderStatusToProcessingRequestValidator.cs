using FastEndpoints;
using FluentValidation;

namespace ODour.Application.Feature.Admin.Order.SwitchOrderStatusToProcessing;

public sealed class SwitchOrderStatusToProcessingRequestValidator
    : Validator<SwitchOrderStatusToProcessingRequest>
{
    public SwitchOrderStatusToProcessingRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(expression: request => request.OrderId).NotEmpty();
    }
}
