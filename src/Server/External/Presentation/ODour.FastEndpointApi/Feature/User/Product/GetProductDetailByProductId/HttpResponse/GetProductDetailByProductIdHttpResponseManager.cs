using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.User.Product.GetProductDetailByProductId;

namespace ODour.FastEndpointApi.Feature.User.Product.GetProductDetailByProductId.HttpResponse;

internal static class GetProductDetailByProductIdHttpResponseManager
{
    private static ConcurrentDictionary<
        GetProductDetailByProductIdResponseStatusCode,
        Func<
            GetProductDetailByProductIdRequest,
            GetProductDetailByProductIdResponse,
            GetProductDetailByProductIdHttpResponse
        >
    > _dictionary;

    private static void Init()
    {
        _dictionary = new();

        _dictionary.TryAdd(
            key: GetProductDetailByProductIdResponseStatusCode.INPUT_VALIDATION_FAIL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: GetProductDetailByProductIdResponseStatusCode.OPERATION_SUCCESS,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status200OK,
                    AppCode = response.StatusCode.ToAppCode(),
                    Body = response.Body
                }
        );

        _dictionary.TryAdd(
            key: GetProductDetailByProductIdResponseStatusCode.PRODUCT_NOT_FOUND,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status404NotFound,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );
    }

    internal static Func<
        GetProductDetailByProductIdRequest,
        GetProductDetailByProductIdResponse,
        GetProductDetailByProductIdHttpResponse
    > Resolve(GetProductDetailByProductIdResponseStatusCode statusCode)
    {
        if (Equals(objA: _dictionary, objB: default))
        {
            Init();
        }

        return _dictionary[statusCode];
    }
}
