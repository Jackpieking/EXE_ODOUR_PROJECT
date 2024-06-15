using System;
using ODour.Application.Share.Features;

namespace ODour.Application.Feature.User.Order.GetUserOrders;

public sealed class GetUserOrdersRequest : IFeatureRequest<GetUserOrdersResponse>
{
    public Guid OrderStatusId { get; init; }

    private Guid _userId;

    public void SetUserId(Guid userId) => _userId = userId;

    public Guid GetUserId() => _userId;
}
