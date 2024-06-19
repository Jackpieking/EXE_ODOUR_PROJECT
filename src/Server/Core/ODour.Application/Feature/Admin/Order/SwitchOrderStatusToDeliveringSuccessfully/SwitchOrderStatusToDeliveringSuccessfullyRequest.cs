using System;
using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Admin.Order.SwitchOrderStatusToDeliveringSuccessfully;

public sealed class SwitchOrderStatusToDeliveringSuccessfullyRequest
    : IFeatureRequest<SwitchOrderStatusToDeliveringSuccessfullyResponse>
{
    public Guid OrderId { get; init; }
}
