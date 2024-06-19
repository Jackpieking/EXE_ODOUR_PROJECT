using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using ODour.Application.Feature.Auth.Logout;
using ODour.Domain.Feature.Main;
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

        #region PreValidateAccessToken
        JsonWebTokenHandler jsonWebTokenHandler = new();

        // Validate access token.
        var validateTokenResult = await jsonWebTokenHandler.ValidateTokenAsync(
            token: context.HttpContext.Request.Headers.Authorization[default].Split(separator: " ")[
                1
            ],
            validationParameters: _tokenValidationParameters.Value
        );

        // Extract and convert access token expire time.
        var tokenExpireTime = ExtractUtcTimeFromToken(context: context.HttpContext);

        // Validate access token.
        if (!validateTokenResult.IsValid || tokenExpireTime <= DateTime.UtcNow)
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

        await using var scope = _serviceScopeFactory.Value.CreateAsyncScope();
        var unitOfWork = scope.Resolve<Lazy<IMainUnitOfWork>>();

        var accessTokenId = Guid.Parse(
                input: context.HttpContext.User.FindFirstValue(
                    claimType: JwtRegisteredClaimNames.Jti
                )
            )
            .ToString();

        // Is refresh token found.
        var isRefreshTokenFound =
            await unitOfWork.Value.LogoutRepository.IsRefreshTokenFoundQueryAsync(
                refreshTokenId: accessTokenId,
                ct: ct
            );

        // Refresh token is not found.
        if (!isRefreshTokenFound)
        {
            await SendResponseAsync(
                statusCode: LogoutResponseStatusCode.FORBIDDEN,
                appRequest: state.AppRequest,
                context: context.HttpContext,
                ct: ct
            );

            return;
        }

        // Set new found refresh token value.
        state.AppRequest.SetRefreshTokenId(refreshTokenId: accessTokenId);
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
