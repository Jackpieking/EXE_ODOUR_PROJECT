using System;
using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Admin.Order.SwitchOrderStatusToCancelling;

public sealed class SwitchOrderStatusToCancellingRequest
    : IFeatureRequest<SwitchOrderStatusToCancellingResponse>
{
    public Guid OrderId { get; init; }
}
