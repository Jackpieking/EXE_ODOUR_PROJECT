using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Feature.Admin.Order.SwitchOrderStatus;
using ODour.Domain.Feature.Main;
using ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatus.Common;

namespace ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatus.Middlewares;

internal sealed class SwitchOrderStatusCachingPreProcessor
    : PreProcessor<SwitchOrderStatusRequest, SwitchOrderStatusStateBag>
{
    private readonly Lazy<IServiceScopeFactory> _serviceScopeFactory;

    public SwitchOrderStatusCachingPreProcessor(Lazy<IServiceScopeFactory> serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override async Task PreProcessAsync(
        IPreProcessorContext<SwitchOrderStatusRequest> context,
        SwitchOrderStatusStateBag state,
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
            await mainUnitOfWork.Value.SwitchOrderStatusRepository.GetOrderCurrentInfoByOrderIdQueryAsync(
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
