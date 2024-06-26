using System;
using System.Threading;
using System.Threading.Tasks;
using ODour.Application.Share.Features;
using ODour.Domain.Feature.Main;

namespace ODour.Application.Feature.User.Cart.RemoveFromCart;

internal sealed class RemoveFromCartHandler
    : IFeatureHandler<RemoveFromCartRequest, RemoveFromCartResponse>
{
    private readonly Lazy<IMainUnitOfWork> _mainUnitOfWork;

    public RemoveFromCartHandler(Lazy<IMainUnitOfWork> mainUnitOfWork)
    {
        _mainUnitOfWork = mainUnitOfWork;
    }

    public async Task<RemoveFromCartResponse> ExecuteAsync(
        RemoveFromCartRequest command,
        CancellationToken ct
    )
    {
        // validate input in database
        #region Validation
        var isInputValid =
            await _mainUnitOfWork.Value.RemoveFromCartRepository.IsInputValidQueryAsync(
                productId: command.ProductId,
                quantity: command.Quantity,
                ct: ct
            );

        if (!isInputValid)
        {
            return new() { StatusCode = RemoveFromCartResponseStatusCode.INPUT_VALIDATION_FAIL };
        }
        #endregion

        // Update quantity again
        var dbResult =
            await _mainUnitOfWork.Value.RemoveFromCartRepository.UpdateQuantityQueryAsync(
                productId: command.ProductId,
                newQuantity: command.Quantity,
                userId: command.GetUserId(),
                ct: ct
            );

        if (!dbResult)
        {
            return new() { StatusCode = RemoveFromCartResponseStatusCode.OPERATION_FAIL };
        }

        // return response
        return new() { StatusCode = RemoveFromCartResponseStatusCode.OPERATION_SUCCESS };
    }
}
