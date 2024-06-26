using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Admin.Order.SwitchOrderStatusToDelivering;

namespace ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToDelivering.HttpResponse;

internal static class SwitchOrderStatusToDeliveringHttpResponseManager
{
    private static ConcurrentDictionary<
        SwitchOrderStatusToDeliveringResponseStatusCode,
        Func<
            SwitchOrderStatusToDeliveringRequest,
            SwitchOrderStatusToDeliveringResponse,
            SwitchOrderStatusToDeliveringHttpResponse
        >
    > _dictionary;

    private static void Init()
    {
        _dictionary = new();

        _dictionary.TryAdd(
            key: SwitchOrderStatusToDeliveringResponseStatusCode.OPERATION_SUCCESS,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status200OK,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: SwitchOrderStatusToDeliveringResponseStatusCode.FORBIDDEN,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status403Forbidden,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: SwitchOrderStatusToDeliveringResponseStatusCode.UN_AUTHORIZED,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status401Unauthorized,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: SwitchOrderStatusToDeliveringResponseStatusCode.INPUT_VALIDATION_FAIL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: SwitchOrderStatusToDeliveringResponseStatusCode.OPERATION_FAIL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status500InternalServerError,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: SwitchOrderStatusToDeliveringResponseStatusCode.ORDER_NOT_FOUND,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status404NotFound,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );
    }

    internal static Func<
        SwitchOrderStatusToDeliveringRequest,
        SwitchOrderStatusToDeliveringResponse,
        SwitchOrderStatusToDeliveringHttpResponse
    > Resolve(SwitchOrderStatusToDeliveringResponseStatusCode statusCode)
    {
        if (Equals(objA: _dictionary, objB: default))
        {
            Init();
        }

        return _dictionary[statusCode];
    }
}
