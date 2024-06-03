using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
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

internal sealed class LogoutAuthorizationPreProcessor : PreProcessor<LogoutRequest, LogoutStateBag>
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
        IPreProcessorContext<LogoutRequest> context,
        LogoutStateBag state,
        CancellationToken ct
    )
    {
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
                context: context,
                ct: ct
            );

            return;
        }

        await using var scope = _serviceScopeFactory.Value.CreateAsyncScope();

        var unitOfWork = scope.Resolve<Lazy<IMainUnitOfWork>>();
        var userManager = scope.Resolve<Lazy<UserManager<UserEntity>>>();

        try
        {
            #region Part1
            // Get refresh token.
            var refreshToken = await unitOfWork.Value.LogoutRepository.GetRefreshTokenQueryAsync(
                refreshToken: context.Request.RefreshToken,
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
                    context: context,
                    ct: ct
                );

                return;
            }

            // Refresh token is expired.
            if (refreshToken.ExpiredAt < DateTime.UtcNow)
            {
                await SendResponseAsync(
                    statusCode: LogoutResponseStatusCode.UN_AUTHORIZED,
                    context: context,
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
                    context: context,
                    ct: ct
                );

                return;
            }
            #endregion

            #region Part3
            // Is user temporarily removed.
            var isUserTemporarilyRemoved =
                await unitOfWork.Value.LogoutRepository.IsUserTemporarilyRemovedQueryAsync(
                    userId: foundUser.Id,
                    ct: ct
                );

            // User is temporarily removed.
            if (isUserTemporarilyRemoved)
            {
                await SendResponseAsync(
                    statusCode: LogoutResponseStatusCode.FORBIDDEN,
                    context: context,
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
                    context: context,
                    ct: ct
                );

                return;
            }
            #endregion
        }
        catch (FormatException)
        {
            await SendResponseAsync(
                statusCode: LogoutResponseStatusCode.FORBIDDEN,
                context: context,
                ct: ct
            );
        }
    }

    private static Task SendResponseAsync(
        LogoutResponseStatusCode statusCode,
        IPreProcessorContext<LogoutRequest> context,
        CancellationToken ct
    )
    {
        var httpResponse = LazyLogoutHttpResponseManager
            .Get()
            .Resolve(statusCode: statusCode)
            .Invoke(arg1: context.Request, arg2: new() { StatusCode = statusCode });

        context.HttpContext.MarkResponseStart();

        return context.HttpContext.Response.SendAsync(
            response: httpResponse,
            statusCode: httpResponse.HttpCode,
            cancellation: ct
        );
    }

    private static bool CheckAccessTokenIsValid(string expClaim)
    {
        return DateTimeOffset.FromUnixTimeSeconds(seconds: long.Parse(s: expClaim)).UtcDateTime
            >= DateTime.UtcNow;
    }
}
