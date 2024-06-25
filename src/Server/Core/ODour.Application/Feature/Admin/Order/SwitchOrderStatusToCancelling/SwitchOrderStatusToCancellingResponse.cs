using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Admin.Order.SwitchOrderStatusToCancelling;

public sealed class SwitchOrderStatusToCancellingResponse : IFeatureResponse
{
    public SwitchOrderStatusToCancellingResponseStatusCode StatusCode { get; init; }
}
