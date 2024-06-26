using System;
using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Admin.Order.SwitchOrderStatusToProcessing;

public sealed class SwitchOrderStatusToProcessingRequest
    : IFeatureRequest<SwitchOrderStatusToProcessingResponse>
{
    public Guid OrderId { get; init; }
}
