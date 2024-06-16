using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Admin.Order.SwitchOrderStatus;

public sealed class SwitchOrderStatusResponse : IFeatureResponse
{
    public SwitchOrderStatusResponseStatusCode StatusCode { get; init; }
}
