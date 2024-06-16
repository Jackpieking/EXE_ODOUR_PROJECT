using System;
using System.Threading;
using System.Threading.Tasks;
using ODour.Application.Share.Features;
using ODour.Domain.Feature.Main;

namespace ODour.Application.Feature.Admin.Order.SwitchOrderStatus;

internal sealed class SwitchOrderStatusHandler
    : IFeatureHandler<SwitchOrderStatusRequest, SwitchOrderStatusResponse>
{
    private readonly Lazy<IMainUnitOfWork> _mainUnitOfWork;

    public SwitchOrderStatusHandler(Lazy<IMainUnitOfWork> mainUnitOfWork)
    {
        _mainUnitOfWork = mainUnitOfWork;
    }

    public async Task<SwitchOrderStatusResponse> ExecuteAsync(
        SwitchOrderStatusRequest command,
        CancellationToken ct
    )
    {
        // Is order find by id.
        var isOrderFound =
            await _mainUnitOfWork.Value.SwitchOrderStatusRepository.IsOrderFoundQueryAsync(
                orderId: command.OrderId,
                ct: ct
            );

        // Order is not found.
        if (!isOrderFound)
        {
            return new() { StatusCode = SwitchOrderStatusResponseStatusCode.ORDER_NOT_FOUND };
        }

        // Is order status Id found.
        var isOrderStatusFound =
            await _mainUnitOfWork.Value.SwitchOrderStatusRepository.IsOrderStatusFoundQueryAsync(
                orderStatusId: command.OrderStatusId,
                ct: ct
            );

        // Order status id is not found.
        if (!isOrderStatusFound)
        {
            return new()
            {
                StatusCode = SwitchOrderStatusResponseStatusCode.ORDER_STATUS_NOT_FOUND
            };
        }

        // Update order status id to the new one.
        var dbResult =
            await _mainUnitOfWork.Value.SwitchOrderStatusRepository.SwitchOrderStatusCommandAsync(
                newOrderStatusId: command.OrderStatusId,
                orderId: command.OrderId,
                ct: ct
            );

        // Cannot update order status.
        if (!dbResult)
        {
            return new() { StatusCode = SwitchOrderStatusResponseStatusCode.OPERATION_FAIL };
        }

        return new() { StatusCode = SwitchOrderStatusResponseStatusCode.OPERATION_SUCCESS };
    }
}
