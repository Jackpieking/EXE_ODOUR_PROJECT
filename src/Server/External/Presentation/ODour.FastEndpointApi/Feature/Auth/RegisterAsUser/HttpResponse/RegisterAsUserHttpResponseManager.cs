using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Auth.RegisterAsUser;

namespace ODour.FastEndpointApi.Feature.Auth.RegisterAsUser.HttpResponse;

internal static class RegisterAsUserHttpResponseManager
{
    private static Dictionary<
        RegisterAsUserResponseStatusCode,
        Func<RegisterAsUserRequest, RegisterAsUserResponse, RegisterAsUserHttpResponse>
    > _dictionary;

    private static void Init()
    {
        _dictionary = new();

        _dictionary.TryAdd(
            key: RegisterAsUserResponseStatusCode.INPUT_VALIDATION_FAIL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: RegisterAsUserResponseStatusCode.OPERATION_FAIL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status500InternalServerError,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: RegisterAsUserResponseStatusCode.OPERATION_SUCCESS,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status200OK,
                    AppCode = response.StatusCode.ToAppCode(),
                }
        );

        _dictionary.TryAdd(
            key: RegisterAsUserResponseStatusCode.USER_IS_EXISTED,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status409Conflict,
                    AppCode = response.StatusCode.ToAppCode(),
                }
        );
    }

    internal static Func<
        RegisterAsUserRequest,
        RegisterAsUserResponse,
        RegisterAsUserHttpResponse
    > Resolve(RegisterAsUserResponseStatusCode statusCode)
    {
        if (Equals(objA: _dictionary, objB: default))
        {
            Init();
        }

        return _dictionary[statusCode];
    }
}
