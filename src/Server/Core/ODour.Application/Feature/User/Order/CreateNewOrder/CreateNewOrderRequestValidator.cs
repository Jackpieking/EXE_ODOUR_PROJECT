using FastEndpoints;
using FluentValidation;
using ODour.Domain.Share.Order.Entities;

namespace ODour.Application.Feature.User.Order.CreateNewOrder;

public sealed class CreateNewOrderRequestValidator : Validator<CreateNewOrderRequest>
{
    public CreateNewOrderRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(expression: request => request.FullName)
            .NotEmpty()
            .MinimumLength(minimumLength: OrderEntity.MetaData.FullName.MinLength)
            .MaximumLength(maximumLength: OrderEntity.MetaData.FullName.MaxLength);

        RuleFor(expression: request => request.PhoneNumber)
            .NotEmpty()
            .MinimumLength(minimumLength: OrderEntity.MetaData.PhoneNumber.MinLength)
            .MaximumLength(maximumLength: OrderEntity.MetaData.PhoneNumber.MaxLength)
            .Matches(expression: @"^(84|0[3|5|7|8|9])+([0-9]{8})\b$");

        RuleFor(expression: request => request.OrderItems).NotEmpty();

        RuleFor(expression: request => request.OrderNote)
            .NotEmpty()
            .MinimumLength(minimumLength: OrderEntity.MetaData.OrderNote.MinLength)
            .MaximumLength(maximumLength: OrderEntity.MetaData.OrderNote.MaxLength);

        RuleFor(expression: request => request.DeliveredAddress)
            .NotEmpty()
            .MinimumLength(minimumLength: OrderEntity.MetaData.DeliveredAddress.MinLength)
            .MaximumLength(maximumLength: OrderEntity.MetaData.DeliveredAddress.MaxLength);

        RuleForEach(expression: request => request.OrderItems)
            .NotEmpty()
            .Must(predicate: childReq => childReq.Quantity > 1);
    }
}
