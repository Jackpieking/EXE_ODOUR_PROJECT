using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Admin.Order.SwitchOrderStatusToCancelling;

namespace ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToCancelling.HttpResponse;

internal static class SwitchOrderStatusToCancellingHttpResponseManager
{
    private static ConcurrentDictionary<
        SwitchOrderStatusToCancellingResponseStatusCode,
        Func<
            SwitchOrderStatusToCancellingRequest,
            SwitchOrderStatusToCancellingResponse,
            SwitchOrderStatusToCancellingHttpResponse
        >
    > _dictionary;

    private static void Init()
    {
        _dictionary = new();

        _dictionary.TryAdd(
            key: SwitchOrderStatusToCancellingResponseStatusCode.OPERATION_SUCCESS,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status200OK,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: SwitchOrderStatusToCancellingResponseStatusCode.FORBIDDEN,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status403Forbidden,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: SwitchOrderStatusToCancellingResponseStatusCode.UN_AUTHORIZED,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status401Unauthorized,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: SwitchOrderStatusToCancellingResponseStatusCode.INPUT_VALIDATION_FAIL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: SwitchOrderStatusToCancellingResponseStatusCode.OPERATION_FAIL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status500InternalServerError,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: SwitchOrderStatusToCancellingResponseStatusCode.ORDER_NOT_FOUND,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status404NotFound,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );
    }

    internal static Func<
        SwitchOrderStatusToCancellingRequest,
        SwitchOrderStatusToCancellingResponse,
        SwitchOrderStatusToCancellingHttpResponse
    > Resolve(SwitchOrderStatusToCancellingResponseStatusCode statusCode)
    {
        if (Equals(objA: _dictionary, objB: default))
        {
            Init();
        }

        return _dictionary[statusCode];
    }
}
