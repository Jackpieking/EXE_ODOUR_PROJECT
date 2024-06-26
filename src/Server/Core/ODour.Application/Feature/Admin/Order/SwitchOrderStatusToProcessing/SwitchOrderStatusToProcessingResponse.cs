using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Admin.Order.SwitchOrderStatusToProcessing;

public sealed class SwitchOrderStatusToProcessingResponse : IFeatureResponse
{
    public SwitchOrderStatusToProcessingResponseStatusCode StatusCode { get; init; }
}
