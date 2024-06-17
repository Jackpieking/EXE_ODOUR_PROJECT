using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Feature.Admin.Order.SwitchOrderStatusToProcessing;
using ODour.Application.Feature.User.Order.GetOrderDetail;
using ODour.Application.Feature.User.Order.GetUserOrders;
using ODour.Application.Share.Caching;
using ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToProcessing.Common;
using ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToProcessing.HttpResponse;

namespace ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToProcessing.Middlewares;

internal sealed class SwitchOrderStatusToProcessingCachingPostProcessor
    : PostProcessor<
        SwitchOrderStatusToProcessingRequest,
        SwitchOrderStatusToProcessingStateBag,
        SwitchOrderStatusToProcessingHttpResponse
    >
{
    private readonly Lazy<IServiceScopeFactory> _serviceScopeFactory;

    public SwitchOrderStatusToProcessingCachingPostProcessor(
        Lazy<IServiceScopeFactory> serviceScopeFactory
    )
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override async Task PostProcessAsync(
        IPostProcessorContext<
            SwitchOrderStatusToProcessingRequest,
            SwitchOrderStatusToProcessingHttpResponse
        > context,
        SwitchOrderStatusToProcessingStateBag state,
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
                value: SwitchOrderStatusToProcessingResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
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
