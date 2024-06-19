using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using ODour.Application.Feature.Auth.RefreshAccessToken;
using ODour.Domain.Feature.Main;
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
        // Bypass if response has started.
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

        // Validate access token expiration.
        if (!validateTokenResult.IsValid)
        {
            await SendResponseAsync(
                statusCode: RefreshAccessTokenResponseStatusCode.UN_AUTHORIZED,
                context: context,
                ct: ct
            );

            return;
        }

        // Extract and convert access token expire time.
        var tokenExpireTime = ExtractUtcTimeFromToken(context: context.HttpContext);

        // Token is not expired.
        if (tokenExpireTime > DateTime.UtcNow)
        {
            await SendResponseAsync(
                statusCode: RefreshAccessTokenResponseStatusCode.FORBIDDEN,
                context: context,
                ct: ct
            );

            return;
        }

        await using var scope = _serviceScopeFactory.Value.CreateAsyncScope();
        var unitOfWork = scope.Resolve<Lazy<IMainUnitOfWork>>();

        // Get id from access token.
        var accessTokenId = Guid.Parse(
            input: context.HttpContext.User.FindFirstValue(claimType: JwtRegisteredClaimNames.Jti)
        );

        // Get refresh token.
        var foundRefreshToken =
            await unitOfWork.Value.RefreshAccessTokenRepository.GetRefreshTokenQueryAsync(
                refreshTokenId: accessTokenId.ToString(),
                refreshTokenValue: context.Request.RefreshToken,
                ct: ct
            );

        // Refresh token is not found.
        if (Equals(objA: foundRefreshToken, objB: default))
        {
            await SendResponseAsync(
                statusCode: RefreshAccessTokenResponseStatusCode.FORBIDDEN,
                context: context,
                ct: ct
            );

            return;
        }

        // State some changes.
        state.FoundUserId = Guid.Parse(
            input: context.HttpContext.User.FindFirstValue(claimType: JwtRegisteredClaimNames.Sub)
        );
        state.FoundAccessTokenId = accessTokenId;
    }

    private static Task SendResponseAsync(
        RefreshAccessTokenResponseStatusCode statusCode,
        IPreProcessorContext<RefreshAccessTokenRequest> context,
        CancellationToken ct
    )
    {
        var httpResponse = RefreshAccessTokenHttpResponseManager
            .Resolve(statusCode: statusCode)
            .Invoke(arg1: context.Request, arg2: new() { StatusCode = statusCode });

        /*
        * Store the real http code of http response into a temporary variable.
        * Set the http code of http response to default for not serializing.
        */
        var httpResponseStatusCode = httpResponse.HttpCode;
        httpResponse.HttpCode = default;

        return context.HttpContext.Response.SendAsync(
            response: httpResponse,
            statusCode: httpResponseStatusCode,
            cancellation: ct
        );
    }

    private static DateTime ExtractUtcTimeFromToken(HttpContext context)
    {
        return DateTimeOffset
            .FromUnixTimeSeconds(
                seconds: long.Parse(
                    s: context.User.FindFirstValue(claimType: JwtRegisteredClaimNames.Exp)
                )
            )
            .UtcDateTime;
    }
}
