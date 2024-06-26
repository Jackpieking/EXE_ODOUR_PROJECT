using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Feature.Admin.Order.SwitchOrderStatusToDeliveringSuccessfully;
using ODour.Application.Feature.User.Order.GetOrderDetail;
using ODour.Application.Feature.User.Order.GetUserOrders;
using ODour.Application.Share.Caching;
using ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToDeliveringSuccessfully.Common;
using ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToDeliveringSuccessfully.HttpResponse;

namespace ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToDeliveringSuccessfully.Middlewares;

internal sealed class SwitchOrderStatusToDeliveringSuccessfullyCachingPostProcessor
    : PostProcessor<
        SwitchOrderStatusToDeliveringSuccessfullyRequest,
        SwitchOrderStatusToDeliveringSuccessfullyStateBag,
        SwitchOrderStatusToDeliveringSuccessfullyHttpResponse
    >
{
    private readonly Lazy<IServiceScopeFactory> _serviceScopeFactory;

    public SwitchOrderStatusToDeliveringSuccessfullyCachingPostProcessor(
        Lazy<IServiceScopeFactory> serviceScopeFactory
    )
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override async Task PostProcessAsync(
        IPostProcessorContext<
            SwitchOrderStatusToDeliveringSuccessfullyRequest,
            SwitchOrderStatusToDeliveringSuccessfullyHttpResponse
        > context,
        SwitchOrderStatusToDeliveringSuccessfullyStateBag state,
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
                value: SwitchOrderStatusToDeliveringSuccessfullyResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
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
