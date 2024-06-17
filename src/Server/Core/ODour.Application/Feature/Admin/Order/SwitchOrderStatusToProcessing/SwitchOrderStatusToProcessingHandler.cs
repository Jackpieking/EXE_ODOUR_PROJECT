using System;
using System.Threading;
using System.Threading.Tasks;
using ODour.Application.Feature.Admin.Order.SwitchOrderStatusToProcessing;
using ODour.Application.Share.Features;
using ODour.Domain.Feature.Main;

namespace ODour.Application.Feature.Admin.Order.SwitchOrderStatusToProcessingToProcessing;

internal sealed class SwitchOrderStatusToProcessingHandler
    : IFeatureHandler<SwitchOrderStatusToProcessingRequest, SwitchOrderStatusToProcessingResponse>
{
    private readonly Lazy<IMainUnitOfWork> _mainUnitOfWork;

    public SwitchOrderStatusToProcessingHandler(Lazy<IMainUnitOfWork> mainUnitOfWork)
    {
        _mainUnitOfWork = mainUnitOfWork;
    }

    public async Task<SwitchOrderStatusToProcessingResponse> ExecuteAsync(
        SwitchOrderStatusToProcessingRequest command,
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
            return new()
            {
                StatusCode = SwitchOrderStatusToProcessingResponseStatusCode.ORDER_NOT_FOUND
            };
        }

        // Update order status id to the new one.
        var dbResult =
            await _mainUnitOfWork.Value.SwitchOrderStatusToProcessingRepository.SwitchOrderStatusCommandAsync(
                orderId: command.OrderId,
                ct: ct
            );

        // Cannot update order status.
        if (!dbResult)
        {
            return new()
            {
                StatusCode = SwitchOrderStatusToProcessingResponseStatusCode.OPERATION_FAIL
            };
        }

        return new()
        {
            StatusCode = SwitchOrderStatusToProcessingResponseStatusCode.OPERATION_SUCCESS
        };
    }
}
