using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Auth.ConfirmUserEmail;

namespace ODour.FastEndpointApi.Feature.Auth.ConfirmUserEmail.HttpResponse;

internal static class ConfirmUserEmailHttpResponseManager
{
    private static ConcurrentDictionary<
        ConfirmUserEmailResponseStatusCode,
        Func<ConfirmUserEmailRequest, ConfirmUserEmailResponse, ConfirmUserEmailHttpResponse>
    > _dictionary;

    private static void Init()
    {
        _dictionary = new();

        _dictionary.TryAdd(
            key: ConfirmUserEmailResponseStatusCode.INPUT_VALIDATION_FAIL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: ConfirmUserEmailResponseStatusCode.OPERATION_FAIL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status500InternalServerError,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: ConfirmUserEmailResponseStatusCode.OPERATION_SUCCESS,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status200OK,
                    AppCode = response.StatusCode.ToAppCode(),
                }
        );

        _dictionary.TryAdd(
            key: ConfirmUserEmailResponseStatusCode.USER_IS_NOT_FOUND,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status404NotFound,
                    AppCode = response.StatusCode.ToAppCode(),
                }
        );

        _dictionary.TryAdd(
            key: ConfirmUserEmailResponseStatusCode.USER_HAS_CONFIRMED_EMAIL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status409Conflict,
                    AppCode = response.StatusCode.ToAppCode(),
                }
        );

        _dictionary.TryAdd(
            key: ConfirmUserEmailResponseStatusCode.USER_IS_TEMPORARILY_BANNED,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status401Unauthorized,
                    AppCode = response.StatusCode.ToAppCode(),
                }
        );
    }

    internal static Func<
        ConfirmUserEmailRequest,
        ConfirmUserEmailResponse,
        ConfirmUserEmailHttpResponse
    > Resolve(ConfirmUserEmailResponseStatusCode statusCode)
    {
        if (Equals(objA: _dictionary, objB: default))
        {
            Init();
        }

        return _dictionary[statusCode];
    }
}
