using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Feature.User.Product.GetRelatedProductsByCategoryId;
using ODour.Application.Share.Caching;
using ODour.FastEndpointApi.Feature.User.Product.GetRelatedProductsByCategoryId.Common;
using ODour.FastEndpointApi.Feature.User.Product.GetRelatedProductsByCategoryId.HttpResponse;

namespace ODour.FastEndpointApi.Feature.User.Product.GetRelatedProductsByCategoryId.Middlewares;

internal sealed class GetRelatedProductsByCategoryIdCachingPreProcessor
    : PreProcessor<GetRelatedProductsByCategoryIdRequest, GetRelatedProductsByCategoryIdStateBag>
{
    private readonly Lazy<IServiceScopeFactory> _serviceScopeFactory;

    public GetRelatedProductsByCategoryIdCachingPreProcessor(
        Lazy<IServiceScopeFactory> serviceScopeFactory
    )
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override async Task PreProcessAsync(
        IPreProcessorContext<GetRelatedProductsByCategoryIdRequest> context,
        GetRelatedProductsByCategoryIdStateBag state,
        CancellationToken ct
    )
    {
        if (context.HttpContext.ResponseStarted())
        {
            return;
        }

        state.CacheKey =
            $"{nameof(GetRelatedProductsByCategoryId)}__req__{context.Request.CategoryId}";

        await using var scope = _serviceScopeFactory.Value.CreateAsyncScope();

        var cacheHandler = scope.Resolve<Lazy<ICacheHandler>>();

        var cacheModel =
            await cacheHandler.Value.GetAsync<GetRelatedProductsByCategoryIdHttpResponse>(
                key: state.CacheKey,
                cancellationToken: ct
            );

        if (
            !Equals(
                objA: cacheModel,
                objB: AppCacheModel<GetRelatedProductsByCategoryIdHttpResponse>.NotFound
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
