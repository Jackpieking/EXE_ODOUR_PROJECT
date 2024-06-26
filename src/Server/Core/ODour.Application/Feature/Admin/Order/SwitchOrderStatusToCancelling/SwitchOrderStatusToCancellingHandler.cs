using System;
using System.Threading;
using System.Threading.Tasks;
using ODour.Application.Share.Features;
using ODour.Domain.Feature.Main;

namespace ODour.Application.Feature.Admin.Order.SwitchOrderStatusToCancelling;

internal sealed class SwitchOrderStatusToCancellingHandler
    : IFeatureHandler<SwitchOrderStatusToCancellingRequest, SwitchOrderStatusToCancellingResponse>
{
    private readonly Lazy<IMainUnitOfWork> _mainUnitOfWork;

    public SwitchOrderStatusToCancellingHandler(Lazy<IMainUnitOfWork> mainUnitOfWork)
    {
        _mainUnitOfWork = mainUnitOfWork;
    }

    public async Task<SwitchOrderStatusToCancellingResponse> ExecuteAsync(
        SwitchOrderStatusToCancellingRequest command,
        CancellationToken ct
    )
    {
        // Is order find by id.
        var isOrderFound =
            await _mainUnitOfWork.Value.SwitchOrderStatusToCancellingRepository.IsOrderFoundQueryAsync(
                orderId: command.OrderId,
                ct: ct
            );

        // Order is not found.
        if (!isOrderFound)
        {
            return new()
            {
                StatusCode = SwitchOrderStatusToCancellingResponseStatusCode.ORDER_NOT_FOUND
            };
        }

        // Update order status id to the new one.
        var dbResult =
            await _mainUnitOfWork.Value.SwitchOrderStatusToCancellingRepository.SwitchOrderStatusCommandAsync(
                orderId: command.OrderId,
                ct: ct
            );

        // Cannot update order status.
        if (!dbResult)
        {
            return new()
            {
                StatusCode = SwitchOrderStatusToCancellingResponseStatusCode.OPERATION_FAIL
            };
        }

        return new()
        {
            StatusCode = SwitchOrderStatusToCancellingResponseStatusCode.OPERATION_SUCCESS
        };
    }
}
