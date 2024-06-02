using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Auth.ConfirmUserEmail;

public sealed class ConfirmUserEmailRequest : IFeatureRequest<ConfirmUserEmailResponse>
{
    public string Token { get; init; }
}
