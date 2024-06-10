using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Feature.User.Cart.AddToCart;
using ODour.Application.Share.Caching;
using ODour.FastEndpointApi.Feature.User.Cart.AddToCart.Common;
using ODour.FastEndpointApi.Feature.User.Cart.AddToCart.HttpResponse;

namespace ODour.FastEndpointApi.Feature.User.Cart.AddToCart.Middlewares;

internal sealed class AddToCartCachingPostProcessor
    : PostProcessor<AddToCartRequest, AddToCartStateBag, AddToCartHttpResponse>
{
    private readonly Lazy<IServiceScopeFactory> _serviceScopeFactory;

    public AddToCartCachingPostProcessor(Lazy<IServiceScopeFactory> serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override async Task PostProcessAsync(
        IPostProcessorContext<AddToCartRequest, AddToCartHttpResponse> context,
        AddToCartStateBag state,
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
                value: AddToCartResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
            )
        )
        {
            // Caching the return value.
            await cacheHandler.Value.RemoveAsync(
                key: $"{nameof(GetCartDetail)}__req__{context.Request.GetUserId()}",
                cancellationToken: ct
            );
        }
    }
}
