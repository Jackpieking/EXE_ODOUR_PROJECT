using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Admin.Order.SwitchOrderStatusToDeliveringSuccessfully;

public sealed class SwitchOrderStatusToDeliveringSuccessfullyResponse : IFeatureResponse
{
    public SwitchOrderStatusToDeliveringSuccessfullyResponseStatusCode StatusCode { get; init; }
}
