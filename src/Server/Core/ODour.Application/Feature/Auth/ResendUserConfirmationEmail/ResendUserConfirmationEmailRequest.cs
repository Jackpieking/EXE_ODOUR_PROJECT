using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Auth.ResendUserConfirmationEmail;

public sealed class ResendUserConfirmationEmailRequest
    : IFeatureRequest<ResendUserConfirmationEmailResponse>
{
    public string Email { get; set; }
}
