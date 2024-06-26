using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Admin.Order.SwitchOrderStatusToDeliveringSuccessfully;

namespace ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToDeliveringSuccessfully.HttpResponse;

internal static class SwitchOrderStatusToDeliveringSuccessfullyHttpResponseManager
{
    private static ConcurrentDictionary<
        SwitchOrderStatusToDeliveringSuccessfullyResponseStatusCode,
        Func<
            SwitchOrderStatusToDeliveringSuccessfullyRequest,
            SwitchOrderStatusToDeliveringSuccessfullyResponse,
            SwitchOrderStatusToDeliveringSuccessfullyHttpResponse
        >
    > _dictionary;

    private static void Init()
    {
        _dictionary = new();

        _dictionary.TryAdd(
            key: SwitchOrderStatusToDeliveringSuccessfullyResponseStatusCode.OPERATION_SUCCESS,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status200OK,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: SwitchOrderStatusToDeliveringSuccessfullyResponseStatusCode.FORBIDDEN,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status403Forbidden,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: SwitchOrderStatusToDeliveringSuccessfullyResponseStatusCode.UN_AUTHORIZED,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status401Unauthorized,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: SwitchOrderStatusToDeliveringSuccessfullyResponseStatusCode.INPUT_VALIDATION_FAIL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: SwitchOrderStatusToDeliveringSuccessfullyResponseStatusCode.OPERATION_FAIL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status500InternalServerError,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: SwitchOrderStatusToDeliveringSuccessfullyResponseStatusCode.ORDER_NOT_FOUND,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status404NotFound,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );
    }

    internal static Func<
        SwitchOrderStatusToDeliveringSuccessfullyRequest,
        SwitchOrderStatusToDeliveringSuccessfullyResponse,
        SwitchOrderStatusToDeliveringSuccessfullyHttpResponse
    > Resolve(SwitchOrderStatusToDeliveringSuccessfullyResponseStatusCode statusCode)
    {
        if (Equals(objA: _dictionary, objB: default))
        {
            Init();
        }

        return _dictionary[statusCode];
    }
}
