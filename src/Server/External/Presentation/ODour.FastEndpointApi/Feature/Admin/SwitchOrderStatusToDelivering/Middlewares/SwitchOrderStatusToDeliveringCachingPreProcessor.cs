using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Feature.Admin.Order.SwitchOrderStatusToDelivering;
using ODour.Domain.Feature.Main;
using ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToDelivering.Common;

namespace ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToDelivering.Middlewares;

internal sealed class SwitchOrderStatusToDeliveringCachingPreProcessor
    : PreProcessor<SwitchOrderStatusToDeliveringRequest, SwitchOrderStatusToDeliveringStateBag>
{
    private readonly Lazy<IServiceScopeFactory> _serviceScopeFactory;

    public SwitchOrderStatusToDeliveringCachingPreProcessor(
        Lazy<IServiceScopeFactory> serviceScopeFactory
    )
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override async Task PreProcessAsync(
        IPreProcessorContext<SwitchOrderStatusToDeliveringRequest> context,
        SwitchOrderStatusToDeliveringStateBag state,
        CancellationToken ct
    )
    {
        if (context.HttpContext.ResponseStarted())
        {
            return;
        }

        await using var scope = _serviceScopeFactory.Value.CreateAsyncScope();

        var mainUnitOfWork = scope.Resolve<Lazy<IMainUnitOfWork>>();

        // Get current order id.
        var foundOrder =
            await mainUnitOfWork.Value.SwitchOrderStatusToDeliveringRepository.GetOrderCurrentInfoByOrderIdQueryAsync(
                orderId: context.Request.OrderId,
                ct: ct
            );

        if (Equals(objA: foundOrder, objB: default))
        {
            state.CurrentOrderStatusId = Guid.Empty;
            state.OrderAuthorId = Guid.Empty;

            return;
        }

        // Assign to state bag.
        state.CurrentOrderStatusId = foundOrder.OrderStatusId;
        state.OrderAuthorId = foundOrder.UserId;
    }
}
