using System;
using ODour.Application.Share.Features;

namespace ODour.Application.Feature.User.Order.GetOrderDetail;

public sealed class GetOrderDetailRequest : IFeatureRequest<GetOrderDetailResponse>
{
    public Guid OrderId { get; init; }
}
