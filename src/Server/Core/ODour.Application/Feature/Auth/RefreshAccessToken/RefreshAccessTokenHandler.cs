using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ODour.Application.Share.Features;
using ODour.Application.Share.Tokens;
using ODour.Domain.Share.User.Entities;

namespace ODour.Application.Feature.Auth.RefreshAccessToken;

internal sealed class RefreshAccessTokenHandler
    : IFeatureHandler<RefreshAccessTokenRequest, RefreshAccessTokenResponse>
{
    private readonly Lazy<UserManager<UserEntity>> _userManager;
    private readonly Lazy<IAccessTokenHandler> _accessTokenHandler;

    public RefreshAccessTokenHandler(
        Lazy<UserManager<UserEntity>> userManager,
        Lazy<IAccessTokenHandler> accessTokenHandler
    )
    {
        _userManager = userManager;
        _accessTokenHandler = accessTokenHandler;
    }

    public async Task<RefreshAccessTokenResponse> ExecuteAsync(
        RefreshAccessTokenRequest command,
        CancellationToken ct
    )
    {
        var foundUser = await _userManager.Value.FindByIdAsync(
            userId: command.GetUserId().ToString()
        );

        var foundUserRoles = await _userManager.Value.GetRolesAsync(user: foundUser);

        // Init list of user claims.
        var userClaims = new List<Claim>
        {
            new(type: "jti", value: Guid.NewGuid().ToString()),
            new(type: "sub", value: foundUser.Id.ToString()),
            new(type: "role", value: foundUserRoles[default])
        };

        // Generate access token.
        var newAccessToken = _accessTokenHandler.Value.GenerateSigningToken(
            claims: userClaims,
            additionalSecondsFromNow: 600
        );

        return new()
        {
            StatusCode = RefreshAccessTokenResponseStatusCode.OPERATION_SUCCESS,
            Body = new() { NewAccessToken = newAccessToken }
        };
    }
}
