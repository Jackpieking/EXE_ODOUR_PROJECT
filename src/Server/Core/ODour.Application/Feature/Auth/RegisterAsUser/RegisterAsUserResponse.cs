using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Auth.RegisterAsUser;

public sealed class RegisterAsUserResponse : IFeatureResponse
{
    public RegisterAsUserResponseStatusCode StatusCode { get; init; }
}
