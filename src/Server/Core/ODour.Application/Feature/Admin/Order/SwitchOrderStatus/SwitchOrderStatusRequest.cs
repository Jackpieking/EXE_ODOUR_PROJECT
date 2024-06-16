using System;
using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Admin.Order.SwitchOrderStatus;

public sealed class SwitchOrderStatusRequest : IFeatureRequest<SwitchOrderStatusResponse>
{
    public Guid OrderId { get; init; }

    public Guid OrderStatusId { get; init; }
}
