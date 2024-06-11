using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Feature.User.Cart.GetCartDetail;
using ODour.Application.Share.Caching;
using ODour.FastEndpointApi.Feature.User.Cart.GetCartDetail.Common;
using ODour.FastEndpointApi.Feature.User.Cart.GetCartDetail.HttpResponse;

namespace ODour.FastEndpointApi.Feature.User.Cart.GetCartDetail.Middlewares;

internal sealed class GetCartDetailCachingPostProcessor
    : PostProcessor<EmptyRequest, GetCartDetailStateBag, GetCartDetailHttpResponse>
{
    private readonly Lazy<IServiceScopeFactory> _serviceScopeFactory;

    public GetCartDetailCachingPostProcessor(Lazy<IServiceScopeFactory> serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override async Task PostProcessAsync(
        IPostProcessorContext<EmptyRequest, GetCartDetailHttpResponse> context,
        GetCartDetailStateBag state,
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
                value: GetCartDetailResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
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
