using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Auth.ConfirmUserEmail;

public sealed class ConfirmUserEmailResponse : IFeatureResponse
{
    public ConfirmUserEmailResponseStatusCode StatusCode { get; set; }
}
