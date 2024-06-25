using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Admin.Order.SwitchOrderStatusToDelivering;

public sealed class SwitchOrderStatusToDeliveringResponse : IFeatureResponse
{
    public SwitchOrderStatusToDeliveringResponseStatusCode StatusCode { get; init; }
}
