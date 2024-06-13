using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Guest.Cart.AddToCart;

namespace ODour.FastEndpointApi.Feature.Guest.Cart.AddToCart.HttpResponse;

internal static class GuestAddToCartHttpResponseManager
{
    private static ConcurrentDictionary<
        GuestAddToCartResponseStatusCode,
        Func<GuestAddToCartRequest, GuestAddToCartResponse, GuestAddToCartHttpResponse>
    > _dictionary;

    private static void Init()
    {
        _dictionary = new();

        _dictionary.TryAdd(
            key: GuestAddToCartResponseStatusCode.OPERATION_SUCCESS,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status200OK,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: GuestAddToCartResponseStatusCode.INPUT_VALIDATION_FAIL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status404NotFound,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: GuestAddToCartResponseStatusCode.CART_IS_FULL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status417ExpectationFailed,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: GuestAddToCartResponseStatusCode.CART_ITEM_QUANTITY_EXCEED,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status417ExpectationFailed,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );
    }

    internal static Func<
        GuestAddToCartRequest,
        GuestAddToCartResponse,
        GuestAddToCartHttpResponse
    > Resolve(GuestAddToCartResponseStatusCode statusCode)
    {
        if (Equals(objA: _dictionary, objB: default))
        {
            Init();
        }

        return _dictionary[statusCode];
    }
}
