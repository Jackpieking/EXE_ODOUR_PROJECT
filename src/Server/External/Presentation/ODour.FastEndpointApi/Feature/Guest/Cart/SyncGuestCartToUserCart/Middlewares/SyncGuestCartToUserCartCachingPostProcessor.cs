using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Feature.Guest.Cart.GetCartDetail;
using ODour.Application.Feature.Guest.Cart.SyncGuestCartToUserCart;
using ODour.Application.Feature.User.Cart.GetCartDetail;
using ODour.Application.Share.Caching;
using ODour.FastEndpointApi.Feature.Guest.Cart.SyncGuestCartToUserCart.Common;
using ODour.FastEndpointApi.Feature.Guest.Cart.SyncGuestCartToUserCart.HttpResponse;

namespace ODour.FastEndpointApi.Feature.Guest.Cart.SyncGuestCartToUserCart.Middlewares;

internal sealed class SyncGuestCartToUserCartCachingPostProcessor
    : PostProcessor<
        EmptyRequest,
        SyncGuestCartToUserCartStateBag,
        SyncGuestCartToUserCartHttpResponse
    >
{
    private readonly Lazy<IServiceScopeFactory> _serviceScopeFactory;

    public SyncGuestCartToUserCartCachingPostProcessor(
        Lazy<IServiceScopeFactory> serviceScopeFactory
    )
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override async Task PostProcessAsync(
        IPostProcessorContext<EmptyRequest, SyncGuestCartToUserCartHttpResponse> context,
        SyncGuestCartToUserCartStateBag state,
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
                value: SyncGuestCartToUserCartResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
            )
        )
        {
            await Task.WhenAll(
                cacheHandler.Value.RemoveAsync(
                    key: $"{nameof(GuestGetCartDetailRequest)}__req__{context.HttpContext.Session.Id}",
                    cancellationToken: ct
                ),
                cacheHandler.Value.RemoveAsync(
                    key: $"{nameof(GetCartDetailRequest)}__req__{state.AppRequest.GetUserId()}",
                    cancellationToken: ct
                )
            );
        }
    }
}
