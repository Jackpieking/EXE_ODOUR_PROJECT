using System;
using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Admin.Order.SwitchOrderStatusToDelivering;

public sealed class SwitchOrderStatusToDeliveringRequest
    : IFeatureRequest<SwitchOrderStatusToDeliveringResponse>
{
    public Guid OrderId { get; init; }
}
