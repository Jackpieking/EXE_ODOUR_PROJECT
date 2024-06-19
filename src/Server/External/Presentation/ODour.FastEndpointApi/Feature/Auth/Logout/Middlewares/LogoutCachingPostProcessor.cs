using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Feature.Admin.Order.SwitchOrderStatusToDeliveringSuccessfully;
using ODour.Application.Feature.Admin.Order.SwitchOrderStatusToProcessing;
using ODour.Application.Feature.Auth.Logout;
using ODour.Application.Feature.Guest.Cart.SyncGuestCartToUserCart;
using ODour.Application.Feature.User.Cart.AddToCart;
using ODour.Application.Feature.User.Cart.GetCartDetail;
using ODour.Application.Feature.User.Cart.RemoveFromCart;
using ODour.Application.Feature.User.Order.CreateNewOrder;
using ODour.Application.Feature.User.Order.GetOrderDetail;
using ODour.Application.Feature.User.Order.GetUserOrders;
using ODour.Application.Share.Caching;
using ODour.FastEndpointApi.Feature.Auth.Logout.Common;
using ODour.FastEndpointApi.Feature.Auth.Logout.HttpResponse;

namespace ODour.FastEndpointApi.Feature.Auth.Logout.Middlewares;

internal sealed class LogoutCachingPostProcessor
    : PostProcessor<EmptyRequest, LogoutStateBag, LogoutHttpResponse>
{
    private readonly Lazy<IServiceScopeFactory> _serviceScopeFactory;

    public LogoutCachingPostProcessor(Lazy<IServiceScopeFactory> serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override async Task PostProcessAsync(
        IPostProcessorContext<EmptyRequest, LogoutHttpResponse> context,
        LogoutStateBag state,
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
                value: LogoutResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
            )
        )
        {
            await Task.WhenAll(
                cacheHandler.Value.RemoveAsync(
                    key: $"{nameof(AddToCartRequest)}__AUTHORIZATION_CHECK__{state.AppRequest.GetRefreshTokenId()}",
                    cancellationToken: ct
                ),
                cacheHandler.Value.RemoveAsync(
                    key: $"{nameof(GetCartDetailRequest)}__AUTHORIZATION_CHECK__{state.AppRequest.GetRefreshTokenId()}",
                    cancellationToken: ct
                ),
                cacheHandler.Value.RemoveAsync(
                    key: $"{nameof(RemoveFromCartRequest)}__AUTHORIZATION_CHECK__{state.AppRequest.GetRefreshTokenId()}",
                    cancellationToken: ct
                ),
                cacheHandler.Value.RemoveAsync(
                    key: $"{nameof(CreateNewOrderRequest)}__AUTHORIZATION_CHECK__{state.AppRequest.GetRefreshTokenId()}",
                    cancellationToken: ct
                ),
                cacheHandler.Value.RemoveAsync(
                    key: $"{nameof(GetOrderDetailRequest)}__AUTHORIZATION_CHECK__{state.AppRequest.GetRefreshTokenId()}",
                    cancellationToken: ct
                ),
                cacheHandler.Value.RemoveAsync(
                    key: $"{nameof(GetUserOrdersRequest)}__AUTHORIZATION_CHECK__{state.AppRequest.GetRefreshTokenId()}",
                    cancellationToken: ct
                ),
                cacheHandler.Value.RemoveAsync(
                    key: $"{nameof(SyncGuestCartToUserCartRequest)}__AUTHORIZATION_CHECK__{state.AppRequest.GetRefreshTokenId()}",
                    cancellationToken: ct
                ),
                cacheHandler.Value.RemoveAsync(
                    key: $"{nameof(SwitchOrderStatusToProcessingRequest)}__AUTHORIZATION_CHECK__{state.AppRequest.GetRefreshTokenId()}",
                    cancellationToken: ct
                ),
                cacheHandler.Value.RemoveAsync(
                    key: $"{nameof(SwitchOrderStatusToDeliveringSuccessfullyRequest)}__AUTHORIZATION_CHECK__{state.AppRequest.GetRefreshTokenId()}",
                    cancellationToken: ct
                )
            );
        }
    }
}
