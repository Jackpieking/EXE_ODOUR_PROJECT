using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.User.Product.GetAllProducts;

namespace ODour.FastEndpointApi.Feature.User.Product.GetAllProducts.HttpResponse;

internal static class GetAllProductsHttpResponseManager
{
    private static ConcurrentDictionary<
        GetAllProductsResponseStatusCode,
        Func<GetAllProductsRequest, GetAllProductsResponse, GetAllProductsHttpResponse>
    > _dictionary;

    private static void Init()
    {
        _dictionary = new();

        _dictionary.TryAdd(
            key: GetAllProductsResponseStatusCode.INPUT_VALIDATION_FAIL,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    AppCode = response.StatusCode.ToAppCode()
                }
        );

        _dictionary.TryAdd(
            key: GetAllProductsResponseStatusCode.OPERATION_SUCCESS,
            value: (_, response) =>
                new()
                {
                    HttpCode = StatusCodes.Status200OK,
                    AppCode = response.StatusCode.ToAppCode(),
                    Body = response.Body
                }
        );
    }

    internal static Func<
        GetAllProductsRequest,
        GetAllProductsResponse,
        GetAllProductsHttpResponse
    > Resolve(GetAllProductsResponseStatusCode statusCode)
    {
        if (Equals(objA: _dictionary, objB: default))
        {
            Init();
        }

        return _dictionary[statusCode];
    }
}
