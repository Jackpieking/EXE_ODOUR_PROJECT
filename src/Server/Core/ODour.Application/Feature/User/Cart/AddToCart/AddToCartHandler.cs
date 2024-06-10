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
        var isInputValid = await _mainUnitOfWork.Value.AddToCartRepository.IsInputValidQueryAsync(
            productId: command.ProductId,
            quantity: command.Quantity,
            ct: ct
        );

        if (!isInputValid)
        {
            return new() { StatusCode = AddToCartResponseStatusCode.INPUT_VALIDATION_FAIL };
        }
        #endregion

        // !!!!!! ONLY WHEN ACCEPT UN-AUTHENTICATED USER.
        // // Find all cart items in the session
        // var session = await _userSession.Value.GetAsync<List<CartItemEntity>>(
        //     key: $"{command.GetUserId()}{CommonConstant.App.DefaultStringSeparator}{CommonConstant.App.AppCartSessionKey}",
        //     ct: ct
        // );

        // List<CartItemEntity> cartItems;

        // // Not initialize the session
        // if (Equals(objA: session, objB: AppSessionModel<List<CartItemEntity>>.NotFound))
        // {
        //     cartItems = new(capacity: 0);
        // }
        // else
        // {
        //     cartItems = session.Value;
        // }

        // // is product in cart
        // var isProductInCart = cartItems.Any(cartItem => cartItem.ProductId == command.ProductId);

        // // Add to cart if not in cart
        // if (!isProductInCart)
        // {
        //     // Get product details
        //     var productDetail =
        //         await _mainUnitOfWork.Value.AddToCartRepository.FindProductQueryAsync(
        //             productId: command.ProductId,
        //             ct: ct
        //         );

        //     // Add to cart
        //     cartItems.Add(
        //         new()
        //         {
        //             ProductId = command.ProductId,
        //             Quantity = command.Quantity,
        //             UserId = command.GetUserId(),
        //             Product = productDetail
        //         }
        //     );
        // }
        // else
        // {
        //     // Update quantity
        //     cartItems
        //         .First(predicate: cartItem => cartItem.ProductId == command.ProductId)
        //         .Quantity += command.Quantity;
        // }

        // // Update session
        // await _userSession.Value.AddAsync(
        //     key: $"{command.GetUserId()}{CommonConstant.App.DefaultStringSeparator}{CommonConstant.App.AppCartSessionKey}",
        //     value: cartItems,
        //     ct: ct
        // );

        // Is cart item found
        var isCartItemFound =
            await _mainUnitOfWork.Value.AddToCartRepository.IsCartItemFoundQueryAsync(
                productId: command.ProductId,
                userId: command.GetUserId(),
                ct: ct
            );

        bool dbResult;

        if (!isCartItemFound)
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
