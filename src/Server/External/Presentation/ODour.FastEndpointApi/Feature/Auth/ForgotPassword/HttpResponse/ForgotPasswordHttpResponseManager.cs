using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Auth.ForgotPassword;

namespace ODour.FastEndpointApi.Feature.Auth.ForgotPassword.HttpResponse;

internal sealed class ForgotPasswordHttpResponseManager
{
    private readonly Dictionary<
        ForgotPasswordResponseStatusCode,
        Func<ForgotPasswordRequest, ForgotPasswordResponse, ForgotPasswordHttpResponse>
    > _dictionary;

    public ForgotPasswordHttpResponseManager()
    {
        _dictionary = new();

        _dictionary.TryAdd(
            key: ForgotPasswordResponseStatusCode.INPUT_VALIDATION_FAIL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: ForgotPasswordResponseStatusCode.OPERATION_FAIL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status500InternalServerError,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: ForgotPasswordResponseStatusCode.OPERATION_SUCCESS,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status200OK,
                    AppCode = response.StatusCode.ToAppCode(),
                }
        );

        _dictionary.TryAdd(
            key: ForgotPasswordResponseStatusCode.USER_IS_NOT_FOUND,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status404NotFound,
                    AppCode = response.StatusCode.ToAppCode(),
                }
        );

        _dictionary.TryAdd(
            key: ForgotPasswordResponseStatusCode.USER_IS_TEMPORARILY_REMOVED,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status401Unauthorized,
                    AppCode = response.StatusCode.ToAppCode(),
                }
        );
    }

    internal Func<
        ForgotPasswordRequest,
        ForgotPasswordResponse,
        ForgotPasswordHttpResponse
    > Resolve(ForgotPasswordResponseStatusCode statusCode)
    {
        return _dictionary[statusCode];
    }
}
