using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using ODour.Application.Feature.Auth.RefreshAccessToken;
using ODour.Domain.Feature.Main;
using ODour.Domain.Share.User.Entities;
using ODour.FastEndpointApi.Feature.Auth.RefreshAccessToken.Common;
using ODour.FastEndpointApi.Feature.Auth.RefreshAccessToken.HttpResponse;

namespace ODour.FastEndpointApi.Feature.Auth.RefreshAccessToken.Middlewares;

internal sealed class RefreshAccessTokenAuthorizationPreProcessor
    : PreProcessor<RefreshAccessTokenRequest, RefreshAccessTokenStateBag>
{
    private readonly Lazy<IServiceScopeFactory> _serviceScopeFactory;
    private readonly Lazy<TokenValidationParameters> _tokenValidationParameters;

    public RefreshAccessTokenAuthorizationPreProcessor(
        Lazy<IServiceScopeFactory> serviceScopeFactory,
        Lazy<TokenValidationParameters> tokenValidationParameters
    )
    {
        _serviceScopeFactory = serviceScopeFactory;
        _tokenValidationParameters = tokenValidationParameters;
    }

    public override async Task PreProcessAsync(
        IPreProcessorContext<RefreshAccessTokenRequest> context,
        RefreshAccessTokenStateBag state,
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

        // Token is not valid.
        if (!validateTokenResult.IsValid)
        {
            await SendResponseAsync(
                statusCode: RefreshAccessTokenResponseStatusCode.FORBIDDEN,
                context: context,
                ct: ct
            );
        }

        await using var scope = _serviceScopeFactory.Value.CreateAsyncScope();

        var unitOfWork = scope.Resolve<Lazy<IMainUnitOfWork>>();
        var userManager = scope.Resolve<Lazy<UserManager<UserEntity>>>();

        try
        {
            #region Part1
            // Does refresh token exist by access token id.
            var isRefreshTokenFound =
                await unitOfWork.Value.RefreshAccessTokenRepository.IsRefreshTokenFoundQueryAsync(
                    refreshToken: context.Request.RefreshToken,
                    refreshTokenId: Guid.Parse(
                            input: context.HttpContext.User.FindFirstValue(
                                claimType: JwtRegisteredClaimNames.Jti
                            )
                        )
                        .ToString(),
                    ct: ct
                );

            // Refresh token is not found by access token id.
            if (!isRefreshTokenFound)
            {
                await SendResponseAsync(
                    statusCode: RefreshAccessTokenResponseStatusCode.FORBIDDEN,
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
                    statusCode: RefreshAccessTokenResponseStatusCode.FORBIDDEN,
                    context: context,
                    ct: ct
                );

                return;
            }
            #endregion

            #region Part3
            // Is user temporarily removed.
            var isUserTemporarilyRemoved =
                await unitOfWork.Value.RefreshAccessTokenRepository.IsUserTemporarilyRemovedQueryAsync(
                    userId: foundUser.Id,
                    ct: ct
                );

            // User is temporarily removed.
            if (isUserTemporarilyRemoved)
            {
                await SendResponseAsync(
                    statusCode: RefreshAccessTokenResponseStatusCode.FORBIDDEN,
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
                    statusCode: RefreshAccessTokenResponseStatusCode.FORBIDDEN,
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
                statusCode: RefreshAccessTokenResponseStatusCode.FORBIDDEN,
                context: context,
                ct: ct
            );
        }
    }

    private static Task SendResponseAsync(
        RefreshAccessTokenResponseStatusCode statusCode,
        IPreProcessorContext<RefreshAccessTokenRequest> context,
        CancellationToken ct
    )
    {
        var httpResponse = LazyRefreshAccessTokenHttpResponseManager
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
}
