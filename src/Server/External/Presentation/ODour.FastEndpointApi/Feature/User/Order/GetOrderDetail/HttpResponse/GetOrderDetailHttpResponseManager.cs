using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.User.Order.GetOrderDetail;

namespace ODour.FastEndpointApi.Feature.User.Order.GetOrderDetail.HttpResponse;

internal static class GetOrderDetailHttpResponseManager
{
    private static ConcurrentDictionary<
        GetOrderDetailResponseStatusCode,
        Func<GetOrderDetailRequest, GetOrderDetailResponse, GetOrderDetailHttpResponse>
    > _dictionary;

    private static void Init()
    {
        _dictionary = new();

        _dictionary.TryAdd(
            key: GetOrderDetailResponseStatusCode.OPERATION_SUCCESS,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status200OK,
                    AppCode = response.StatusCode.ToAppCode(),
                    Body = response.Body
                }
        );

        _dictionary.TryAdd(
            key: GetOrderDetailResponseStatusCode.FORBIDDEN,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status403Forbidden,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: GetOrderDetailResponseStatusCode.UN_AUTHORIZED,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status401Unauthorized,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: GetOrderDetailResponseStatusCode.INPUT_VALIDATION_FAIL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: GetOrderDetailResponseStatusCode.ORDER_NOT_FOUND,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status404NotFound,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );
    }

    internal static Func<
        GetOrderDetailRequest,
        GetOrderDetailResponse,
        GetOrderDetailHttpResponse
    > Resolve(GetOrderDetailResponseStatusCode statusCode)
    {
        if (Equals(objA: _dictionary, objB: default))
        {
            Init();
        }

        return _dictionary[statusCode];
    }
}
