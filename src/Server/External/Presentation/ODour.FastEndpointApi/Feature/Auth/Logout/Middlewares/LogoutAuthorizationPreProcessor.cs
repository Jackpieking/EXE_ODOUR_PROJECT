using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using ODour.Application.Feature.Auth.Logout;
using ODour.Domain.Feature.Main;
using ODour.Domain.Share.User.Entities;
using ODour.FastEndpointApi.Feature.Auth.Logout.Common;
using ODour.FastEndpointApi.Feature.Auth.Logout.HttpResponse;

namespace ODour.FastEndpointApi.Feature.Auth.Logout.Middlewares;

internal sealed class LogoutAuthorizationPreProcessor : PreProcessor<EmptyRequest, LogoutStateBag>
{
    private readonly Lazy<IServiceScopeFactory> _serviceScopeFactory;
    private readonly Lazy<TokenValidationParameters> _tokenValidationParameters;

    public LogoutAuthorizationPreProcessor(
        Lazy<IServiceScopeFactory> serviceScopeFactory,
        Lazy<TokenValidationParameters> tokenValidationParameters
    )
    {
        _serviceScopeFactory = serviceScopeFactory;
        _tokenValidationParameters = tokenValidationParameters;
    }

    public override async Task PreProcessAsync(
        IPreProcessorContext<EmptyRequest> context,
        LogoutStateBag state,
        CancellationToken ct
    )
    {
        // Bypass if response has started.
        if (context.HttpContext.ResponseStarted())
        {
            return;
        }

        // Set new app request.
        state.AppRequest = new();

        JsonWebTokenHandler jsonWebTokenHandler = new();

        // Validate access token.
        var validateTokenResult = await jsonWebTokenHandler.ValidateTokenAsync(
            token: context.HttpContext.Request.Headers.Authorization[default].Split(separator: " ")[
                1
            ],
            validationParameters: _tokenValidationParameters.Value
        );

        // Validate access token.
        if (
            !validateTokenResult.IsValid
            || !CheckAccessTokenIsValid(
                expClaim: context.HttpContext.User.FindFirstValue(
                    claimType: JwtRegisteredClaimNames.Exp
                )
            )
        )
        {
            await SendResponseAsync(
                statusCode: LogoutResponseStatusCode.UN_AUTHORIZED,
                appRequest: state.AppRequest,
                context: context.HttpContext,
                ct: ct
            );

            return;
        }

        await using var scope = _serviceScopeFactory.Value.CreateAsyncScope();

        var unitOfWork = scope.Resolve<Lazy<IMainUnitOfWork>>();
        var userManager = scope.Resolve<Lazy<UserManager<UserEntity>>>();

        #region Part1
        // Get refresh token.
        var refreshToken = await unitOfWork.Value.LogoutRepository.GetRefreshTokenQueryAsync(
            refreshTokenId: Guid.Parse(
                    input: context.HttpContext.User.FindFirstValue(
                        claimType: JwtRegisteredClaimNames.Jti
                    )
                )
                .ToString(),
            ct: ct
        );

        // Refresh token is not found.
        if (Equals(objA: refreshToken, objB: default))
        {
            await SendResponseAsync(
                statusCode: LogoutResponseStatusCode.FORBIDDEN,
                appRequest: state.AppRequest,
                context: context.HttpContext,
                ct: ct
            );

            return;
        }

        // Refresh token is expired.
        if (refreshToken.ExpiredAt < DateTime.UtcNow)
        {
            await SendResponseAsync(
                statusCode: LogoutResponseStatusCode.UN_AUTHORIZED,
                appRequest: state.AppRequest,
                context: context.HttpContext,
                ct: ct
            );

            return;
        }
        #endregion

        #region Part2
        // Find user by user id.
        var foundUser = await userManager.Value.FindByIdAsync(
            userId: Guid.Parse(
                    input: context.HttpContext.User.FindFirstValue(
                        claimType: JwtRegisteredClaimNames.Sub
                    )
                )
                .ToString()
        );

        // User is not found
        if (Equals(objA: foundUser, objB: default))
        {
            await SendResponseAsync(
                statusCode: LogoutResponseStatusCode.FORBIDDEN,
                appRequest: state.AppRequest,
                context: context.HttpContext,
                ct: ct
            );

            return;
        }
        #endregion

        #region Part3
        // Is user temporarily removed.
        var isUserTemporarilyRemoved =
            await unitOfWork.Value.LogoutRepository.IsUserBannedQueryAsync(
                userId: foundUser.Id,
                ct: ct
            );

        // User is temporarily removed.
        if (isUserTemporarilyRemoved)
        {
            await SendResponseAsync(
                statusCode: LogoutResponseStatusCode.FORBIDDEN,
                appRequest: state.AppRequest,
                context: context.HttpContext,
                ct: ct
            );

            return;
        }
        #endregion

        #region Part4
        // Is user in role.
        var isUserInRole = await userManager.Value.IsInRoleAsync(
            user: foundUser,
            role: context.HttpContext.User.FindFirstValue(claimType: "role")
        );

        // User is not in role.
        if (!isUserInRole)
        {
            await SendResponseAsync(
                statusCode: LogoutResponseStatusCode.FORBIDDEN,
                appRequest: state.AppRequest,
                context: context.HttpContext,
                ct: ct
            );

            return;
        }
        #endregion

        // Set new found refresh token value.
        state.FoundRefreshTokenValue = refreshToken.Value;
    }

    private static Task SendResponseAsync(
        LogoutResponseStatusCode statusCode,
        LogoutRequest appRequest,
        HttpContext context,
        CancellationToken ct
    )
    {
        var httpResponse = LogoutHttpResponseManager
            .Resolve(statusCode: statusCode)
            .Invoke(arg1: appRequest, arg2: new() { StatusCode = statusCode });

        /*
        * Store the real http code of http response into a temporary variable.
        * Set the http code of http response to default for not serializing.
        */
        var httpResponseStatusCode = httpResponse.HttpCode;
        httpResponse.HttpCode = default;

        return context.Response.SendAsync(
            response: httpResponse,
            statusCode: httpResponseStatusCode,
            cancellation: ct
        );
    }

    private static bool CheckAccessTokenIsValid(string expClaim)
    {
        return DateTimeOffset.FromUnixTimeSeconds(seconds: long.Parse(s: expClaim)).UtcDateTime
            >= DateTime.UtcNow;
    }
}
