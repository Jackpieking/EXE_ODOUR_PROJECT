using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Feature.User.Product.GetProductsForHomePage;
using ODour.Application.Share.Caching;
using ODour.FastEndpointApi.Feature.User.Product.GetProductsForHomePage.Common;
using ODour.FastEndpointApi.Feature.User.Product.GetProductsForHomePage.HttpResponse;

namespace ODour.FastEndpointApi.Feature.User.Product.GetProductsForHomePage.Middlewares;

internal sealed class GetProductsForHomePageCachingPostProcessor
    : PostProcessor<
        EmptyRequest,
        GetProductsForHomePageStateBag,
        GetProductsForHomePageHttpResponse
    >
{
    private readonly Lazy<IServiceScopeFactory> _serviceScopeFactory;

    public GetProductsForHomePageCachingPostProcessor(
        Lazy<IServiceScopeFactory> serviceScopeFactory
    )
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override async Task PostProcessAsync(
        IPostProcessorContext<EmptyRequest, GetProductsForHomePageHttpResponse> context,
        GetProductsForHomePageStateBag state,
        CancellationToken ct
    )
    {
        if (Equals(objA: context.Response, objB: default))
        {
            return;
        }

        await using var scope = _serviceScopeFactory.Value.CreateAsyncScope();

        var cacheHandler = scope.Resolve<Lazy<ICacheHandler>>();

        // Set new cache if current app code is suitable.
        if (
            context.Response.AppCode.Equals(
                value: GetProductsForHomePageResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
            )
        )
        {
            // Caching the return value.
            await cacheHandler.Value.SetAsync(
                key: state.CacheKey,
                value: context.Response,
                new()
                {
                    AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(
                        seconds: state.CacheDurationInSeconds
                    )
                },
                cancellationToken: ct
            );
        }
    }
}
