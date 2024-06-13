using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Feature.User.Cart.GetCartDetail;
using ODour.Application.Feature.User.Cart.RemoveFromCart;
using ODour.Application.Share.Caching;
using ODour.FastEndpointApi.Feature.User.Cart.RemoveFromCart.Common;
using ODour.FastEndpointApi.Feature.User.Cart.RemoveFromCart.HttpResponse;

namespace ODour.FastEndpointApi.Feature.User.Cart.RemoveFromCart.Middlewares;

internal sealed class RemoveFromCartCachingPostProcessor
    : PostProcessor<RemoveFromCartRequest, RemoveFromCartStateBag, RemoveFromCartHttpResponse>
{
    private readonly Lazy<IServiceScopeFactory> _serviceScopeFactory;

    public RemoveFromCartCachingPostProcessor(Lazy<IServiceScopeFactory> serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override async Task PostProcessAsync(
        IPostProcessorContext<RemoveFromCartRequest, RemoveFromCartHttpResponse> context,
        RemoveFromCartStateBag state,
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
                value: RemoveFromCartResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
            )
        )
        {
            // Caching the return value.
            await cacheHandler.Value.RemoveAsync(
                key: $"{nameof(GetCartDetailRequest)}__req__{context.Request.GetUserId()}",
                cancellationToken: ct
            );
        }
    }
}
