using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Feature.User.Order.GetOrderDetail;
using ODour.Application.Share.Caching;
using ODour.FastEndpointApi.Feature.User.Order.GetOrderDetail.Common;
using ODour.FastEndpointApi.Feature.User.Order.GetOrderDetail.HttpResponse;

namespace ODour.FastEndpointApi.Feature.User.Order.GetOrderDetail.Middlewares;

internal sealed class GetOrderDetailCachingPostProcessor
    : PostProcessor<GetOrderDetailRequest, GetOrderDetailStateBag, GetOrderDetailHttpResponse>
{
    private readonly Lazy<IServiceScopeFactory> _serviceScopeFactory;

    public GetOrderDetailCachingPostProcessor(Lazy<IServiceScopeFactory> serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override async Task PostProcessAsync(
        IPostProcessorContext<GetOrderDetailRequest, GetOrderDetailHttpResponse> context,
        GetOrderDetailStateBag state,
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
                value: GetOrderDetailResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
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
