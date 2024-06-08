using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.User.Product.GetRelatedProductsByCategoryId;

namespace ODour.FastEndpointApi.Feature.User.Product.GetRelatedProductsByCategoryId.HttpResponse;

internal static class GetRelatedProductsByCategoryIdHttpResponseManager
{
    private static ConcurrentDictionary<
        GetRelatedProductsByCategoryIdResponseStatusCode,
        Func<
            GetRelatedProductsByCategoryIdRequest,
            GetRelatedProductsByCategoryIdResponse,
            GetRelatedProductsByCategoryIdHttpResponse
        >
    > _dictionary;

    private static void Init()
    {
        _dictionary = new();

        _dictionary.TryAdd(
            key: GetRelatedProductsByCategoryIdResponseStatusCode.INPUT_VALIDATION_FAIL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: GetRelatedProductsByCategoryIdResponseStatusCode.OPERATION_SUCCESS,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status200OK,
                    AppCode = response.StatusCode.ToAppCode(),
                    Body = response.Body
                }
        );

        _dictionary.TryAdd(
            key: GetRelatedProductsByCategoryIdResponseStatusCode.CATEGORY_NOT_FOUND,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status404NotFound,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );
    }

    internal static Func<
        GetRelatedProductsByCategoryIdRequest,
        GetRelatedProductsByCategoryIdResponse,
        GetRelatedProductsByCategoryIdHttpResponse
    > Resolve(GetRelatedProductsByCategoryIdResponseStatusCode statusCode)
    {
        if (Equals(objA: _dictionary, objB: default))
        {
            Init();
        }

        return _dictionary[statusCode];
    }
}
