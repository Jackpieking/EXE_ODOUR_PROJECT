using System;
using System.Threading;
using System.Threading.Tasks;
using ODour.Application.Share.Features;
using ODour.Domain.Feature.Main;

namespace ODour.Application.Feature.Admin.Order.SwitchOrderStatusToDeliveringSuccessfully;

internal sealed class SwitchOrderStatusToDeliveringSuccessfullyHandler
    : IFeatureHandler<
        SwitchOrderStatusToDeliveringSuccessfullyRequest,
        SwitchOrderStatusToDeliveringSuccessfullyResponse
    >
{
    private readonly Lazy<IMainUnitOfWork> _mainUnitOfWork;

    public SwitchOrderStatusToDeliveringSuccessfullyHandler(Lazy<IMainUnitOfWork> mainUnitOfWork)
    {
        _mainUnitOfWork = mainUnitOfWork;
    }

    public async Task<SwitchOrderStatusToDeliveringSuccessfullyResponse> ExecuteAsync(
        SwitchOrderStatusToDeliveringSuccessfullyRequest command,
        CancellationToken ct
    )
    {
        // Is order find by id.
        var isOrderFound =
            await _mainUnitOfWork.Value.SwitchOrderStatusToDeliveringSuccessfullyRepository.IsOrderFoundQueryAsync(
                orderId: command.OrderId,
                ct: ct
            );

        // Order is not found.
        if (!isOrderFound)
        {
            return new()
            {
                StatusCode =
                    SwitchOrderStatusToDeliveringSuccessfullyResponseStatusCode.ORDER_NOT_FOUND
            };
        }

        // Update order status id to the new one.
        var dbResult =
            await _mainUnitOfWork.Value.SwitchOrderStatusToDeliveringSuccessfullyRepository.SwitchOrderStatusCommandAsync(
                orderId: command.OrderId,
                ct: ct
            );

        // Cannot update order status.
        if (!dbResult)
        {
            return new()
            {
                StatusCode =
                    SwitchOrderStatusToDeliveringSuccessfullyResponseStatusCode.OPERATION_FAIL
            };
        }

        return new()
        {
            StatusCode =
                SwitchOrderStatusToDeliveringSuccessfullyResponseStatusCode.OPERATION_SUCCESS
        };
    }
}
