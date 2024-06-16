using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Guest.Cart.GetCartDetail;

namespace ODour.FastEndpointApi.Feature.Guest.Cart.GetCartDetail.HttpResponse;

internal static class GuestGetCartDetailHttpResponseManager
{
    private static ConcurrentDictionary<
        GuestGetCartDetailResponseStatusCode,
        Func<GuestGetCartDetailRequest, GuestGetCartDetailResponse, GuestGetCartDetailHttpResponse>
    > _dictionary;

    private static void Init()
    {
        _dictionary = new();

        _dictionary.TryAdd(
            key: GuestGetCartDetailResponseStatusCode.OPERATION_SUCCESS,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status200OK,
                    AppCode = response.StatusCode.ToAppCode(),
                    Body = response.Body
                }
        );

        _dictionary.TryAdd(
            key: GuestGetCartDetailResponseStatusCode.INPUT_VALIDATION_FAIL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );
    }

    internal static Func<
        GuestGetCartDetailRequest,
        GuestGetCartDetailResponse,
        GuestGetCartDetailHttpResponse
    > Resolve(GuestGetCartDetailResponseStatusCode statusCode)
    {
        if (Equals(objA: _dictionary, objB: default))
        {
            Init();
        }

        return _dictionary[statusCode];
    }
}
