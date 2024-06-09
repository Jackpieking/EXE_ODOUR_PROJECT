using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.User.Product.GetProductsForHomePage;

namespace ODour.FastEndpointApi.Feature.User.Product.GetProductsForHomePage.HttpResponse;

internal static class GetProductsForHomePageHttpResponseManager
{
    private static ConcurrentDictionary<
        GetProductsForHomePageResponseStatusCode,
        Func<
            GetProductsForHomePageRequest,
            GetProductsForHomePageResponse,
            GetProductsForHomePageHttpResponse
        >
    > _dictionary;

    private static void Init()
    {
        _dictionary = new();

        _dictionary.TryAdd(
            key: GetProductsForHomePageResponseStatusCode.OPERATION_SUCCESS,
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
        GetProductsForHomePageRequest,
        GetProductsForHomePageResponse,
        GetProductsForHomePageHttpResponse
    > Resolve(GetProductsForHomePageResponseStatusCode statusCode)
    {
        if (Equals(objA: _dictionary, objB: default))
        {
            Init();
        }

        return _dictionary[statusCode];
    }
}
