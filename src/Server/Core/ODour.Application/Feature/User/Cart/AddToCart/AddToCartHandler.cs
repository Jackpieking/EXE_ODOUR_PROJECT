using System;
using System.Threading;
using System.Threading.Tasks;
using ODour.Application.Share.Features;
using ODour.Domain.Feature.Main;

namespace ODour.Application.Feature.User.Cart.AddToCart;

internal sealed class AddToCartHandler : IFeatureHandler<AddToCartRequest, AddToCartResponse>
{
    private readonly Lazy<IMainUnitOfWork> _mainUnitOfWork;

    public AddToCartHandler(Lazy<IMainUnitOfWork> mainUnitOfWork)
    {
        _mainUnitOfWork = mainUnitOfWork;
    }

    public async Task<AddToCartResponse> ExecuteAsync(
        AddToCartRequest command,
        CancellationToken ct
    )
    {
        // Validate input at database
        #region Validation
        var foundProduct = await _mainUnitOfWork.Value.AddToCartRepository.FindProductQueryAsync(
            productId: command.ProductId,
            ct: ct
        );

        if (
            Equals(objA: foundProduct, objB: default)
            || command.Quantity > foundProduct.QuantityInStock
        )
        {
            return new() { StatusCode = AddToCartResponseStatusCode.INPUT_VALIDATION_FAIL };
        }

        // Is cart item found
        var foundCartItem = await _mainUnitOfWork.Value.AddToCartRepository.FindCartItemQueryAsync(
            productId: command.ProductId,
            ct: ct
        );

        if (
            !Equals(objA: foundCartItem, objB: default)
            && foundCartItem.Quantity + command.Quantity > foundProduct.QuantityInStock
        )
        {
            return new() { StatusCode = AddToCartResponseStatusCode.INPUT_VALIDATION_FAIL };
        }
        #endregion

        bool dbResult;

        if (Equals(objA: foundCartItem, objB: default))
        {
            // Add to cart
            dbResult = await _mainUnitOfWork.Value.AddToCartRepository.AddItemToCartQueryAsync(
                cartItem: new()
                {
                    ProductId = command.ProductId,
                    Quantity = command.Quantity,
                    UserId = command.GetUserId()
                },
                ct: ct
            );
        }
        else
        {
            // Update quantity
            dbResult = await _mainUnitOfWork.Value.AddToCartRepository.UpdateQuantityQueryAsync(
                productId: command.ProductId,
                newQuantity: command.Quantity,
                userId: command.GetUserId(),
                ct: ct
            );
        }

        // Add to cart fail
        if (!dbResult)
        {
            return new() { StatusCode = AddToCartResponseStatusCode.OPERATION_FAIL };
        }

        return new() { StatusCode = AddToCartResponseStatusCode.OPERATION_SUCCESS };
    }
}
