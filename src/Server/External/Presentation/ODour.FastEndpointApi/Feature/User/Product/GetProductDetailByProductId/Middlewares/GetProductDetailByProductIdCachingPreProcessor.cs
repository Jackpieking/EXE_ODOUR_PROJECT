using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Feature.User.Product.GetProductDetailByProductId;
using ODour.Application.Share.Caching;
using ODour.FastEndpointApi.Feature.User.Product.GetProductDetailByProductId.Common;
using ODour.FastEndpointApi.Feature.User.Product.GetProductDetailByProductId.HttpResponse;

namespace ODour.FastEndpointApi.Feature.User.Product.GetProductDetailByProductId.Middlewares;

internal sealed class GetProductDetailByProductIdCachingPreProcessor
    : PreProcessor<GetProductDetailByProductIdRequest, GetProductDetailByProductIdStateBag>
{
    private readonly Lazy<IServiceScopeFactory> _serviceScopeFactory;

    public GetProductDetailByProductIdCachingPreProcessor(
        Lazy<IServiceScopeFactory> serviceScopeFactory
    )
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override async Task PreProcessAsync(
        IPreProcessorContext<GetProductDetailByProductIdRequest> context,
        GetProductDetailByProductIdStateBag state,
        CancellationToken ct
    )
    {
        if (context.HttpContext.ResponseStarted())
        {
            return;
        }

        state.CacheKey =
            $"{nameof(GetProductDetailByProductIdRequest)}__req__{context.Request.ProductId}";

        await using var scope = _serviceScopeFactory.Value.CreateAsyncScope();

        var cacheHandler = scope.Resolve<Lazy<ICacheHandler>>();

        var cacheModel = await cacheHandler.Value.GetAsync<GetProductDetailByProductIdHttpResponse>(
            key: state.CacheKey,
            cancellationToken: ct
        );

        if (
            !Equals(
                objA: cacheModel,
                objB: AppCacheModel<GetProductDetailByProductIdHttpResponse>.NotFound
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
