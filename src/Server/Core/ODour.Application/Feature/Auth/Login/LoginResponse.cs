using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Auth.Login;

public sealed class LoginResponse : IFeatureResponse
{
    public LoginResponseStatusCode StatusCode { get; init; }

    public ResponseBody Body { get; init; }

    public sealed class ResponseBody
    {
        public string AccessToken { get; init; }

        public string RefreshToken { get; init; }

        public UserCredential User { get; init; }

        public sealed class UserCredential
        {
            public string Email { get; init; }

            public bool EmailConfirmed { get; init; }

            public string AvatarUrl { get; init; }
        }
    }
}
