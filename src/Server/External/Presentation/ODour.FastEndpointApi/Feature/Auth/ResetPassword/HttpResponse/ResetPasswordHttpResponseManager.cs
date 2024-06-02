using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Auth.ResetPassword;

namespace ODour.FastEndpointApi.Feature.Auth.ResetPassword.HttpResponse;

internal sealed class ResetPasswordHttpResponseManager
{
    private readonly Dictionary<
        ResetPasswordResponseStatusCode,
        Func<ResetPasswordRequest, ResetPasswordResponse, ResetPasswordHttpResponse>
    > _dictionary;

    public ResetPasswordHttpResponseManager()
    {
        _dictionary = new();

        _dictionary.TryAdd(
            key: ResetPasswordResponseStatusCode.INPUT_VALIDATION_FAIL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: ResetPasswordResponseStatusCode.OPERATION_FAIL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status500InternalServerError,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: ResetPasswordResponseStatusCode.OPERATION_SUCCESS,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status200OK,
                    AppCode = response.StatusCode.ToAppCode(),
                }
        );

        _dictionary.TryAdd(
            key: ResetPasswordResponseStatusCode.USER_IS_NOT_FOUND,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status404NotFound,
                    AppCode = response.StatusCode.ToAppCode(),
                }
        );

        _dictionary.TryAdd(
            key: ResetPasswordResponseStatusCode.USER_IS_TEMPORARILY_REMOVED,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status401Unauthorized,
                    AppCode = response.StatusCode.ToAppCode(),
                }
        );
    }

    internal Func<ResetPasswordRequest, ResetPasswordResponse, ResetPasswordHttpResponse> Resolve(
        ResetPasswordResponseStatusCode statusCode
    )
    {
        return _dictionary[statusCode];
    }
}
