using FastEndpoints;
using FluentValidation;

namespace ODour.Application.Feature.User.Order.GetUserOrders;

public sealed class GetUserOrdersRequestValidator : Validator<GetUserOrdersRequest>
{
    public GetUserOrdersRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(expression: request => request.OrderStatusId).NotEmpty();
    }
}
