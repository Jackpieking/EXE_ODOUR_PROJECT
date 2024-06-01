using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Auth.ResendUserConfirmationEmail;

public sealed class ResendUserConfirmationEmailResponse : IFeatureResponse
{
    public ResendUserConfirmationEmailResponseStatusCode StatusCode { get; set; }
}
