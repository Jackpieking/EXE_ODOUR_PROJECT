using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Auth.RegisterAsAdmin;

public sealed class RegisterAsAdminResponse : IFeatureResponse
{
    public RegisterAsAdminResponseStatusCode StatusCode { get; init; }
}
