using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Auth.ResendUserConfirmationEmail;

namespace ODour.FastEndpointApi.Feature.Auth.ResendUserConfirmationEmail.HttpResponse;

internal sealed class ResendUserConfirmationEmailHttpResponseManager
{
    private readonly Dictionary<
        ResendUserConfirmationEmailResponseStatusCode,
        Func<
            ResendUserConfirmationEmailRequest,
            ResendUserConfirmationEmailResponse,
            ResendUserConfirmationEmailHttpResponse
        >
    > _dictionary;

    public ResendUserConfirmationEmailHttpResponseManager()
    {
        _dictionary = new();

        _dictionary.TryAdd(
            key: ResendUserConfirmationEmailResponseStatusCode.INPUT_VALIDATION_FAIL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: ResendUserConfirmationEmailResponseStatusCode.OPERATION_FAIL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status500InternalServerError,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: ResendUserConfirmationEmailResponseStatusCode.OPERATION_SUCCESS,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status200OK,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: ResendUserConfirmationEmailResponseStatusCode.USER_IS_NOT_FOUND,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status404NotFound,
                    AppCode = response.StatusCode.ToAppCode(),
                }
        );

        _dictionary.TryAdd(
            key: ResendUserConfirmationEmailResponseStatusCode.USER_HAS_CONFIRMED_EMAIL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status409Conflict,
                    AppCode = response.StatusCode.ToAppCode(),
                }
        );

        _dictionary.TryAdd(
            key: ResendUserConfirmationEmailResponseStatusCode.USER_IS_TEMPORARILY_BANNED,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status401Unauthorized,
                    AppCode = response.StatusCode.ToAppCode(),
                }
        );
    }

    internal Func<
        ResendUserConfirmationEmailRequest,
        ResendUserConfirmationEmailResponse,
        ResendUserConfirmationEmailHttpResponse
    > Resolve(ResendUserConfirmationEmailResponseStatusCode statusCode)
    {
        return _dictionary[statusCode];
    }
}
