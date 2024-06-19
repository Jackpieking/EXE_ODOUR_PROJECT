﻿using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using ODour.Application.Feature.User.Order.CreateNewOrder;
using ODour.Application.Share.Caching;
using ODour.Domain.Feature.Main;
using ODour.FastEndpointApi.Feature.User.Order.CreateNewOrder.Common;
using ODour.FastEndpointApi.Feature.User.Order.CreateNewOrder.HttpResponse;

namespace ODour.FastEndpointApi.Feature.User.Order.CreateNewOrder.Middlewares;

internal sealed class CreateNewOrderAuthorizationPreProcessor
    : PreProcessor<CreateNewOrderRequest, CreateNewOrderStateBag>
{
    private readonly Lazy<IServiceScopeFactory> _serviceScopeFactory;
    private readonly Lazy<TokenValidationParameters> _tokenValidationParameters;

    public CreateNewOrderAuthorizationPreProcessor(
        Lazy<IServiceScopeFactory> serviceScopeFactory,
        Lazy<TokenValidationParameters> tokenValidationParameters
    )
    {
        _serviceScopeFactory = serviceScopeFactory;
        _tokenValidationParameters = tokenValidationParameters;
    }

    public override async Task PreProcessAsync(
        IPreProcessorContext<CreateNewOrderRequest> context,
        CreateNewOrderStateBag state,
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

        var dateTimeUtcNow = DateTime.UtcNow;

        // Validate access token.
        if (!validateTokenResult.IsValid || tokenExpireTime <= dateTimeUtcNow)
        {
            await SendResponseAsync(
                statusCode: CreateNewOrderResponseStatusCode.UN_AUTHORIZED,
                appRequest: context.Request,
                context: context.HttpContext,
                ct: ct
            );

            return;
        }
        #endregion

        await using var scope = _serviceScopeFactory.Value.CreateAsyncScope();
        var cacheHandler = scope.Resolve<Lazy<ICacheHandler>>();
        var unitOfWork = scope.Resolve<Lazy<IMainUnitOfWork>>();

        // Get id from access token.
        var accessTokenId = Guid.Parse(
                input: context.HttpContext.User.FindFirstValue(
                    claimType: JwtRegisteredClaimNames.Jti
                )
            )
            .ToString();

        var cacheKey = $"{nameof(CreateNewOrderRequest)}__AUTHORIZATION_CHECK__{accessTokenId}";

        // Get authorized access token id from cache.
        var foundAuthorizedAccessToken = await cacheHandler.Value.GetAsync<bool>(
            key: cacheKey,
            cancellationToken: ct
        );

        // Authorized access token id is found.
        if (Equals(objA: foundAuthorizedAccessToken, objB: AppCacheModel<bool>.NotFound))
        {
            // Is refresh token found.
            var isRefreshTokenFound =
                await unitOfWork.Value.CreateNewOrderRepository.IsRefreshTokenFoundQueryAsync(
                    refreshTokenId: accessTokenId,
                    ct: ct
                );

            // Refresh token is not found.
            if (!isRefreshTokenFound)
            {
                await SendResponseAsync(
                    statusCode: CreateNewOrderResponseStatusCode.FORBIDDEN,
                    appRequest: context.Request,
                    context: context.HttpContext,
                    ct: ct
                );

                return;
            }

            var cacheExpiredTime = tokenExpireTime - dateTimeUtcNow - TimeSpan.FromSeconds(15);

            // Caching the authorized access token id for faster authorization.
            // Let the cache expire sooner than 15 seconds from
            // the access token expire time.
            await cacheHandler.Value.SetAsync(
                key: cacheKey,
                value: true,
                new() { AbsoluteExpiration = dateTimeUtcNow.Add(value: cacheExpiredTime) },
                cancellationToken: ct
            );
        }

        // Set user id.
        context.Request.SetUserId(
            userId: Guid.Parse(
                input: context.HttpContext.User.FindFirstValue(
                    claimType: JwtRegisteredClaimNames.Sub
                )
            )
        );
    }

    private static Task SendResponseAsync(
        CreateNewOrderResponseStatusCode statusCode,
        CreateNewOrderRequest appRequest,
        HttpContext context,
        CancellationToken ct
    )
    {
        var httpResponse = CreateNewOrderHttpResponseManager
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
