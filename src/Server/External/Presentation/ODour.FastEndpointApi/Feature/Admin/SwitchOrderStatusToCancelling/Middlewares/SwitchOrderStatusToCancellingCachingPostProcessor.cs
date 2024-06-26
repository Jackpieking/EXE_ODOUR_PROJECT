using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Feature.Admin.Order.SwitchOrderStatusToCancelling;
using ODour.Application.Feature.User.Order.GetOrderDetail;
using ODour.Application.Feature.User.Order.GetUserOrders;
using ODour.Application.Share.Caching;
using ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToCancelling.Common;
using ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToCancelling.HttpResponse;

namespace ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToCancelling.Middlewares;

internal sealed class SwitchOrderStatusToCancellingCachingPostProcessor
    : PostProcessor<
        SwitchOrderStatusToCancellingRequest,
        SwitchOrderStatusToCancellingStateBag,
        SwitchOrderStatusToCancellingHttpResponse
    >
{
    private readonly Lazy<IServiceScopeFactory> _serviceScopeFactory;

    public SwitchOrderStatusToCancellingCachingPostProcessor(
        Lazy<IServiceScopeFactory> serviceScopeFactory
    )
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override async Task PostProcessAsync(
        IPostProcessorContext<
            SwitchOrderStatusToCancellingRequest,
            SwitchOrderStatusToCancellingHttpResponse
        > context,
        SwitchOrderStatusToCancellingStateBag state,
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
                value: SwitchOrderStatusToCancellingResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
            )
        )
        {
            await Task.WhenAll(
                cacheHandler.Value.RemoveAsync(
                    key: $"{nameof(GetOrderDetailRequest)}__req__{context.Request.OrderId}",
                    cancellationToken: ct
                ),
                cacheHandler.Value.RemoveAsync(
                    key: $"{nameof(GetUserOrdersRequest)}__req__{state.OrderAuthorId}__{state.CurrentOrderStatusId}",
                    cancellationToken: ct
                )
            );
        }
    }
}
