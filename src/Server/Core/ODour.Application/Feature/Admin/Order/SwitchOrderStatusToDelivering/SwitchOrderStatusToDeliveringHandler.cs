using System;
using System.Threading;
using System.Threading.Tasks;
using ODour.Application.Share.Features;
using ODour.Domain.Feature.Main;

namespace ODour.Application.Feature.Admin.Order.SwitchOrderStatusToDelivering;

internal sealed class SwitchOrderStatusToDeliveringHandler
    : IFeatureHandler<SwitchOrderStatusToDeliveringRequest, SwitchOrderStatusToDeliveringResponse>
{
    private readonly Lazy<IMainUnitOfWork> _mainUnitOfWork;

    public SwitchOrderStatusToDeliveringHandler(Lazy<IMainUnitOfWork> mainUnitOfWork)
    {
        _mainUnitOfWork = mainUnitOfWork;
    }

    public async Task<SwitchOrderStatusToDeliveringResponse> ExecuteAsync(
        SwitchOrderStatusToDeliveringRequest command,
        CancellationToken ct
    )
    {
        // Is order find by id.
        var isOrderFound =
            await _mainUnitOfWork.Value.SwitchOrderStatusToDeliveringRepository.IsOrderFoundQueryAsync(
                orderId: command.OrderId,
                ct: ct
            );

        // Order is not found.
        if (!isOrderFound)
        {
            return new()
            {
                StatusCode = SwitchOrderStatusToDeliveringResponseStatusCode.ORDER_NOT_FOUND
            };
        }

        // Update order status id to the new one.
        var dbResult =
            await _mainUnitOfWork.Value.SwitchOrderStatusToDeliveringRepository.SwitchOrderStatusCommandAsync(
                orderId: command.OrderId,
                ct: ct
            );

        // Cannot update order status.
        if (!dbResult)
        {
            return new()
            {
                StatusCode = SwitchOrderStatusToDeliveringResponseStatusCode.OPERATION_FAIL
            };
        }

        return new()
        {
            StatusCode = SwitchOrderStatusToDeliveringResponseStatusCode.OPERATION_SUCCESS
        };
    }
}
