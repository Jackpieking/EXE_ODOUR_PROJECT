using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Feature.User.Product.GetAllProducts;
using ODour.Application.Share.Caching;
using ODour.FastEndpointApi.Feature.User.Product.GetAllProducts.Common;
using ODour.FastEndpointApi.Feature.User.Product.GetAllProducts.HttpResponse;

namespace ODour.FastEndpointApi.Feature.User.Product.GetAllProducts.Middlewares;

internal sealed class GetAllProductsCachingPreProcessor
    : PreProcessor<GetAllProductsRequest, GetAllProductsStateBag>
{
    private readonly Lazy<IServiceScopeFactory> _serviceScopeFactory;

    public GetAllProductsCachingPreProcessor(Lazy<IServiceScopeFactory> serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override async Task PreProcessAsync(
        IPreProcessorContext<GetAllProductsRequest> context,
        GetAllProductsStateBag state,
        CancellationToken ct
    )
    {
        if (context.HttpContext.ResponseStarted())
        {
            return;
        }

        state.CacheKey = nameof(GetAllProducts);

        await using var scope = _serviceScopeFactory.Value.CreateAsyncScope();

        var cacheHandler = scope.Resolve<Lazy<ICacheHandler>>();

        var cacheModel = await cacheHandler.Value.GetAsync<GetAllProductsHttpResponse>(
            key: state.CacheKey,
            cancellationToken: ct
        );

        if (!Equals(objA: cacheModel, objB: AppCacheModel<GetAllProductsHttpResponse>.NotFound))
        {
            var httpCode = cacheModel.Value.HttpCode;
            cacheModel.Value.HttpCode = default;

            await context.HttpContext.Response.SendAsync(
                response: cacheModel.Value,
                statusCode: httpCode,
                cancellation: ct
            );

            context.HttpContext.MarkResponseStart();

            return;
        }
    }
}
