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

internal sealed class GetProductsForHomePageCachingPreProcessor
    : PreProcessor<EmptyRequest, GetProductsForHomePageStateBag>
{
    private readonly Lazy<IServiceScopeFactory> _serviceScopeFactory;

    public GetProductsForHomePageCachingPreProcessor(Lazy<IServiceScopeFactory> serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override async Task PreProcessAsync(
        IPreProcessorContext<EmptyRequest> context,
        GetProductsForHomePageStateBag state,
        CancellationToken ct
    )
    {
        if (context.HttpContext.ResponseStarted())
        {
            return;
        }

        state.CacheKey = nameof(GetProductsForHomePageRequest);

        await using var scope = _serviceScopeFactory.Value.CreateAsyncScope();

        var cacheHandler = scope.Resolve<Lazy<ICacheHandler>>();

        var cacheModel = await cacheHandler.Value.GetAsync<GetProductsForHomePageHttpResponse>(
            key: state.CacheKey,
            cancellationToken: ct
        );

        if (
            !Equals(
                objA: cacheModel,
                objB: AppCacheModel<GetProductsForHomePageHttpResponse>.NotFound
            )
        )
        {
            var httpCode = cacheModel.Value.HttpCode;
            cacheModel.Value.HttpCode = default;

            await context.HttpContext.Response.SendAsync(
                response: cacheModel.Value,
                statusCode: httpCode,
                cancellation: ct
            );

            return;
        }
    }
}
