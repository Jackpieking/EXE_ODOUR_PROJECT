using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Auth.RegisterAsAdmin;

namespace ODour.FastEndpointApi.Feature.Auth.RegisterAsAdmin.HttpResponse;

internal sealed class RegisterAsAdminHttpResponseManager
{
    private readonly Dictionary<
        RegisterAsAdminResponseStatusCode,
        Func<RegisterAsAdminRequest, RegisterAsAdminResponse, RegisterAsAdminHttpResponse>
    > _dictionary;

    public RegisterAsAdminHttpResponseManager()
    {
        _dictionary = new();

        _dictionary.TryAdd(
            key: RegisterAsAdminResponseStatusCode.INPUT_VALIDATION_FAIL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: RegisterAsAdminResponseStatusCode.OPERATION_FAIL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status500InternalServerError,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: RegisterAsAdminResponseStatusCode.OPERATION_SUCCESS,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status200OK,
                    AppCode = response.StatusCode.ToAppCode(),
                }
        );

        _dictionary.TryAdd(
            key: RegisterAsAdminResponseStatusCode.USER_IS_EXISTED,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status409Conflict,
                    AppCode = response.StatusCode.ToAppCode(),
                }
        );

        _dictionary.TryAdd(
            key: RegisterAsAdminResponseStatusCode.FORBIDDEN,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status403Forbidden,
                    AppCode = response.StatusCode.ToAppCode(),
                }
        );
    }

    internal Func<
        RegisterAsAdminRequest,
        RegisterAsAdminResponse,
        RegisterAsAdminHttpResponse
    > Resolve(RegisterAsAdminResponseStatusCode statusCode)
    {
        return _dictionary[statusCode];
    }
}
