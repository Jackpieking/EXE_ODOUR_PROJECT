using System;
using ODour.Application.Share.Features;

namespace ODour.Application.Feature.User.Order.CreateNewOrder;

public sealed class CreateNewOrderResponse : IFeatureResponse
{
    public CreateNewOrderResponseStatusCode StatusCode { get; init; }

    public Guid OrderStatusId { get; init; }
}
