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
using ODour.Application.Feature.User.Cart.RemoveFromCart;
using ODour.Domain.Feature.Main;
using ODour.Domain.Share.User.Entities;
using ODour.FastEndpointApi.Feature.User.Cart.RemoveFromCart.Common;
using ODour.FastEndpointApi.Feature.User.Cart.RemoveFromCart.HttpResponse;

namespace ODour.FastEndpointApi.Feature.User.Cart.RemoveFromCart.Middlewares;

internal sealed class RemoveFromCartAuthorizationPreProcessor
    : PreProcessor<RemoveFromCartRequest, RemoveFromCartStateBag>
{
    private readonly Lazy<IServiceScopeFactory> _serviceScopeFactory;
    private readonly Lazy<TokenValidationParameters> _tokenValidationParameters;

    public RemoveFromCartAuthorizationPreProcessor(
        Lazy<IServiceScopeFactory> serviceScopeFactory,
        Lazy<TokenValidationParameters> tokenValidationParameters
    )
    {
        _serviceScopeFactory = serviceScopeFactory;
        _tokenValidationParameters = tokenValidationParameters;
    }

    public override async Task PreProcessAsync(
        IPreProcessorContext<RemoveFromCartRequest> context,
        RemoveFromCartStateBag state,
        CancellationToken ct
    )
    {
        if (context.HttpContext.ResponseStarted())
        {
            return;
        }

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
                statusCode: RemoveFromCartResponseStatusCode.UN_AUTHORIZED,
                appRequest: context.Request,
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
        var refreshToken =
            await unitOfWork.Value.RemoveFromCartRepository.GetRefreshTokenQueryAsync(
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
                statusCode: RemoveFromCartResponseStatusCode.FORBIDDEN,
                appRequest: context.Request,
                context: context.HttpContext,
                ct: ct
            );

            return;
        }

        // Refresh token is expired.
        if (refreshToken.ExpiredAt < DateTime.UtcNow)
        {
            await SendResponseAsync(
                statusCode: RemoveFromCartResponseStatusCode.UN_AUTHORIZED,
                appRequest: context.Request,
                context: context.HttpContext,
                ct: ct
            );

            return;
        }
        #endregion

        #region Part2
        var foundUserId = Guid.Parse(
            input: context.HttpContext.User.FindFirstValue(claimType: JwtRegisteredClaimNames.Sub)
        );

        // Find user by user id.
        var foundUser = await userManager.Value.FindByIdAsync(userId: foundUserId.ToString());

        // User is not found
        if (Equals(objA: foundUser, objB: default))
        {
            await SendResponseAsync(
                statusCode: RemoveFromCartResponseStatusCode.FORBIDDEN,
                appRequest: context.Request,
                context: context.HttpContext,
                ct: ct
            );

            return;
        }
        #endregion

        #region Part3
        // Is user temporarily removed.
        var isUserTemporarilyRemoved =
            await unitOfWork.Value.RemoveFromCartRepository.IsUserBannedQueryAsync(
                userId: foundUser.Id,
                ct: ct
            );

        // User is temporarily removed.
        if (isUserTemporarilyRemoved)
        {
            await SendResponseAsync(
                statusCode: RemoveFromCartResponseStatusCode.FORBIDDEN,
                appRequest: context.Request,
                context: context.HttpContext,
                ct: ct
            );

            return;
        }
        #endregion

        #region Part4
        var roleClaim = context.HttpContext.User.FindFirstValue(claimType: "role");

        // Is user in role
        if (
            !roleClaim.Equals(value: "user")
            || !(await userManager.Value.IsInRoleAsync(user: foundUser, role: roleClaim))
        )
        {
            await SendResponseAsync(
                statusCode: RemoveFromCartResponseStatusCode.FORBIDDEN,
                appRequest: context.Request,
                context: context.HttpContext,
                ct: ct
            );

            return;
        }
        #endregion

        // Set user id.
        context.Request.SetUserId(userId: foundUser.Id);
    }

    private static Task SendResponseAsync(
        RemoveFromCartResponseStatusCode statusCode,
        RemoveFromCartRequest appRequest,
        HttpContext context,
        CancellationToken ct
    )
    {
        var httpResponse = RemoveFromCartHttpResponseManager
            .Resolve(statusCode: statusCode)
            .Invoke(arg1: appRequest, arg2: new() { StatusCode = statusCode });

        context.MarkResponseStart();

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
