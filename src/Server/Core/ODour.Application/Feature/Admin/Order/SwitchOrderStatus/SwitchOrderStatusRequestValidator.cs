using FastEndpoints;
using FluentValidation;

namespace ODour.Application.Feature.Admin.Order.SwitchOrderStatus;

public sealed class SwitchOrderStatusRequestValidator : Validator<SwitchOrderStatusRequest>
{
    public SwitchOrderStatusRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(expression: request => request.OrderId).NotEmpty();

        RuleFor(expression: request => request.OrderStatusId).NotEmpty();
    }
}
