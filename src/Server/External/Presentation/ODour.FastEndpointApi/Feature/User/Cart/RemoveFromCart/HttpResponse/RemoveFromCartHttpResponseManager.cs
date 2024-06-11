using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.User.Cart.RemoveFromCart;

namespace ODour.FastEndpointApi.Feature.User.Cart.RemoveFromCart.HttpResponse;

internal static class RemoveFromCartHttpResponseManager
{
    private static ConcurrentDictionary<
        RemoveFromCartResponseStatusCode,
        Func<RemoveFromCartRequest, RemoveFromCartResponse, RemoveFromCartHttpResponse>
    > _dictionary;

    private static void Init()
    {
        _dictionary = new();

        _dictionary.TryAdd(
            key: RemoveFromCartResponseStatusCode.OPERATION_SUCCESS,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status200OK,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: RemoveFromCartResponseStatusCode.FORBIDDEN,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status403Forbidden,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: RemoveFromCartResponseStatusCode.UN_AUTHORIZED,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status401Unauthorized,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: RemoveFromCartResponseStatusCode.INPUT_VALIDATION_FAIL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status404NotFound,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: RemoveFromCartResponseStatusCode.OPERATION_FAIL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status500InternalServerError,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );
    }

    internal static Func<
        RemoveFromCartRequest,
        RemoveFromCartResponse,
        RemoveFromCartHttpResponse
    > Resolve(RemoveFromCartResponseStatusCode statusCode)
    {
        if (Equals(objA: _dictionary, objB: default))
        {
            Init();
        }

        return _dictionary[statusCode];
    }
}
