using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using FastEndpoints.Security;
using Microsoft.IdentityModel.Tokens;
using ODour.Application.Share.Tokens.AccessToken;
using ODour.Configuration.Presentation.WebApi.Authentication;

namespace ODour.AppIdentityService.Handlers;

/// <summary>
///     Implementation of jwt generator interface.
/// </summary>
internal sealed class AppAccessTokenHandler : IAccessTokenHandler
{
    private readonly Lazy<JwtAuthenticationOption> _jwtAuthenticationOption;

    public AppAccessTokenHandler(Lazy<JwtAuthenticationOption> jwtAuthenticationOption)
    {
        _jwtAuthenticationOption = jwtAuthenticationOption;
    }

    public string GenerateSigningToken(IEnumerable<Claim> claims)
    {
        // Validate set of user claims.
        if (claims.Equals(obj: Enumerable.Empty<Claim>()) || Equals(objA: claims, objB: default))
        {
            return string.Empty;
        }

        return JwtBearer.CreateToken(options: option =>
        {
            option.SigningKey = _jwtAuthenticationOption.Value.Jwt.IssuerSigningKey;
            option.ExpireAt = DateTime.UtcNow.AddMinutes(value: 15);
            option.User.Claims.AddRange(collection: claims);
            option.Audience = _jwtAuthenticationOption.Value.Jwt.ValidAudience;
            option.Issuer = _jwtAuthenticationOption.Value.Jwt.ValidIssuer;
            option.SigningAlgorithm = SecurityAlgorithms.HmacSha256;
            option.CompressionAlgorithm = CompressionAlgorithms.Deflate;
            option.User.Claims.Add(
                item: new(
                    type: JwtRegisteredClaimNames.Iat,
                    value: DateTime.UtcNow.ToLongTimeString()
                )
            );
        });
    }
}
