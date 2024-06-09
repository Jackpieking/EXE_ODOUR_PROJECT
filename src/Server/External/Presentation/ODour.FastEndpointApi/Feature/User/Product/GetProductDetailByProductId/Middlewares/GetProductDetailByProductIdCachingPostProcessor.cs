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

internal sealed class GetProductDetailByProductIdCachingPostProcessor
    : PostProcessor<
        GetProductDetailByProductIdRequest,
        GetProductDetailByProductIdStateBag,
        GetProductDetailByProductIdHttpResponse
    >
{
    private readonly Lazy<IServiceScopeFactory> _serviceScopeFactory;

    public GetProductDetailByProductIdCachingPostProcessor(
        Lazy<IServiceScopeFactory> serviceScopeFactory
    )
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override async Task PostProcessAsync(
        IPostProcessorContext<
            GetProductDetailByProductIdRequest,
            GetProductDetailByProductIdHttpResponse
        > context,
        GetProductDetailByProductIdStateBag state,
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
                value: GetProductDetailByProductIdResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
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
