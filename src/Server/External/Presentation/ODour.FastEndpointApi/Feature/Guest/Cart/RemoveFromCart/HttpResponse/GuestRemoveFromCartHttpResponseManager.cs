using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Guest.Cart.RemoveFromCart;

namespace ODour.FastEndpointApi.Feature.Guest.Cart.RemoveFromCart.HttpResponse;

internal static class GuestRemoveFromCartHttpResponseManager
{
    private static ConcurrentDictionary<
        GuestRemoveFromCartResponseStatusCode,
        Func<
            GuestRemoveFromCartRequest,
            GuestRemoveFromCartResponse,
            GuestRemoveFromCartHttpResponse
        >
    > _dictionary;

    private static void Init()
    {
        _dictionary = new();

        _dictionary.TryAdd(
            key: GuestRemoveFromCartResponseStatusCode.OPERATION_SUCCESS,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status200OK,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: GuestRemoveFromCartResponseStatusCode.INPUT_VALIDATION_FAIL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );
    }

    internal static Func<
        GuestRemoveFromCartRequest,
        GuestRemoveFromCartResponse,
        GuestRemoveFromCartHttpResponse
    > Resolve(GuestRemoveFromCartResponseStatusCode statusCode)
    {
        if (Equals(objA: _dictionary, objB: default))
        {
            Init();
        }

        return _dictionary[statusCode];
    }
}
