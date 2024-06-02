using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Auth.RefreshAccessToken;

namespace ODour.FastEndpointApi.Feature.Auth.RefreshAccessToken.HttpResponse;

internal sealed class RefreshAccessTokenHttpResponseManager
{
    private readonly Dictionary<
        RefreshAccessTokenResponseStatusCode,
        Func<RefreshAccessTokenRequest, RefreshAccessTokenResponse, RefreshAccessTokenHttpResponse>
    > _dictionary;

    public RefreshAccessTokenHttpResponseManager()
    {
        _dictionary = new();

        _dictionary.TryAdd(
            key: RefreshAccessTokenResponseStatusCode.INPUT_VALIDATION_FAIL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: RefreshAccessTokenResponseStatusCode.OPERATION_SUCCESS,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status200OK,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: RefreshAccessTokenResponseStatusCode.FORBIDDEN,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status403Forbidden,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: RefreshAccessTokenResponseStatusCode.UN_AUTHORIZED,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status401Unauthorized,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );
    }

    internal Func<
        RefreshAccessTokenRequest,
        RefreshAccessTokenResponse,
        RefreshAccessTokenHttpResponse
    > Resolve(RefreshAccessTokenResponseStatusCode statusCode)
    {
        return _dictionary[statusCode];
    }
}
