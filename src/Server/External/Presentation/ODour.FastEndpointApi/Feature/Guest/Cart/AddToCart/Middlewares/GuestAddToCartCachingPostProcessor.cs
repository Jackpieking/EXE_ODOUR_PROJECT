using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Feature.Guest.Cart.AddToCart;
using ODour.Application.Feature.Guest.Cart.GetCartDetail;
using ODour.Application.Share.Caching;
using ODour.FastEndpointApi.Feature.Guest.Cart.AddToCart.Common;
using ODour.FastEndpointApi.Feature.Guest.Cart.AddToCart.HttpResponse;

namespace ODour.FastEndpointApi.Feature.Guest.Cart.AddToCart.Middlewares;

internal sealed class GuestAddToCartCachingPostProcessor
    : PostProcessor<GuestAddToCartRequest, GuestAddToCartStateBag, GuestAddToCartHttpResponse>
{
    private readonly Lazy<IServiceScopeFactory> _serviceScopeFactory;

    public GuestAddToCartCachingPostProcessor(Lazy<IServiceScopeFactory> serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override async Task PostProcessAsync(
        IPostProcessorContext<GuestAddToCartRequest, GuestAddToCartHttpResponse> context,
        GuestAddToCartStateBag state,
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
                value: GuestAddToCartResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
            )
        )
        {
            await cacheHandler.Value.RemoveAsync(
                key: $"{nameof(GuestGetCartDetailRequest)}__req__{context.HttpContext.Session.Id}",
                cancellationToken: ct
            );
        }
    }
}
