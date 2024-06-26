using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Admin.Order.SwitchOrderStatusToProcessing;

namespace ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToProcessing.HttpResponse;

internal static class SwitchOrderStatusToProcessingHttpResponseManager
{
    private static ConcurrentDictionary<
        SwitchOrderStatusToProcessingResponseStatusCode,
        Func<
            SwitchOrderStatusToProcessingRequest,
            SwitchOrderStatusToProcessingResponse,
            SwitchOrderStatusToProcessingHttpResponse
        >
    > _dictionary;

    private static void Init()
    {
        _dictionary = new();

        _dictionary.TryAdd(
            key: SwitchOrderStatusToProcessingResponseStatusCode.OPERATION_SUCCESS,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status200OK,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: SwitchOrderStatusToProcessingResponseStatusCode.FORBIDDEN,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status403Forbidden,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: SwitchOrderStatusToProcessingResponseStatusCode.UN_AUTHORIZED,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status401Unauthorized,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: SwitchOrderStatusToProcessingResponseStatusCode.INPUT_VALIDATION_FAIL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: SwitchOrderStatusToProcessingResponseStatusCode.OPERATION_FAIL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status500InternalServerError,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: SwitchOrderStatusToProcessingResponseStatusCode.ORDER_NOT_FOUND,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status404NotFound,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );
    }

    internal static Func<
        SwitchOrderStatusToProcessingRequest,
        SwitchOrderStatusToProcessingResponse,
        SwitchOrderStatusToProcessingHttpResponse
    > Resolve(SwitchOrderStatusToProcessingResponseStatusCode statusCode)
    {
        if (Equals(objA: _dictionary, objB: default))
        {
            Init();
        }

        return _dictionary[statusCode];
    }
}
