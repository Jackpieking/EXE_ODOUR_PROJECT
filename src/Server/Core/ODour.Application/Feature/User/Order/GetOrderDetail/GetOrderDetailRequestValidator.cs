using FastEndpoints;
using FluentValidation;

namespace ODour.Application.Feature.User.Order.GetOrderDetail;

public sealed class GetOrderDetailRequestValidator : Validator<GetOrderDetailRequest>
{
    public GetOrderDetailRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(expression: request => request.OrderId).NotEmpty();
    }
}
