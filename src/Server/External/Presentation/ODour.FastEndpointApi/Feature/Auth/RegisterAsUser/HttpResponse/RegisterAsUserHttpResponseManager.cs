using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Auth.RegisterAsUser;

namespace ODour.FastEndpointApi.Feature.Auth.RegisterAsUser.HttpResponse;

internal sealed class RegisterAsUserHttpResponseManager
{
    private readonly ConcurrentDictionary<
        RegisterAsUserResponseStatusCode,
        Func<RegisterAsUserRequest, RegisterAsUserResponse, RegisterAsUserHttpResponse>
    > _dictionary;

    internal RegisterAsUserHttpResponseManager()
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

    internal Func<
        RegisterAsUserRequest,
        RegisterAsUserResponse,
        RegisterAsUserHttpResponse
    > Resolve(RegisterAsUserResponseStatusCode statusCode)
    {
        return _dictionary[statusCode];
    }
}
