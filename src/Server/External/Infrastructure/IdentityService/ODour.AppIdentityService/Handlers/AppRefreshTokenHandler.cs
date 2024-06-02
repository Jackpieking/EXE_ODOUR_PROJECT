using System.Security.Cryptography;
using System.Text;
using ODour.Application.Share.Tokens;

namespace ODour.AppIdentityService.Handlers;

/// <summary>
///     Implementation refresh token generator interface.
/// </summary>
internal sealed class AppRefreshTokenHandler : IRefreshTokenHandler
{
    public string Generate(int length)
    {
        const int RefreshTokenMaxLength = 10;
        const string Chars =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890abcdefghijklmnopqrstuvwxyz!@#$%^&*+=";

        if (length < RefreshTokenMaxLength)
        {
            return string.Empty;
        }

        StringBuilder builder = new();

        for (int time = default; time < length; time++)
        {
            builder.Append(
                value: Chars[index: RandomNumberGenerator.GetInt32(toExclusive: Chars.Length)]
            );
        }

        return builder.ToString();
    }
}
