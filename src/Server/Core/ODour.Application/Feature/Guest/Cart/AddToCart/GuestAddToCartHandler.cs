using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ODour.Application.Share.Common;
using ODour.Application.Share.Features;
using ODour.Application.Share.Session;
using ODour.Domain.Feature.Main;
using ODour.Domain.Share.Cart.Entities;

namespace ODour.Application.Feature.Guest.Cart.AddToCart;

internal sealed class GuestAddToCartHandler
    : IFeatureHandler<GuestAddToCartRequest, GuestAddToCartResponse>
{
    private readonly Lazy<IMainUnitOfWork> _mainUnitOfWork;
    private readonly Lazy<IUserSession> _userSession;

    public GuestAddToCartHandler(
        Lazy<IMainUnitOfWork> mainUnitOfWork,
        Lazy<IUserSession> userSession
    )
    {
        _mainUnitOfWork = mainUnitOfWork;
        _userSession = userSession;
    }

    public async Task<GuestAddToCartResponse> ExecuteAsync(
        GuestAddToCartRequest command,
        CancellationToken ct
    )
    {
        // find product.
        var foundProduct =
            await _mainUnitOfWork.Value.GuestAddToCartRepository.GetProductQuantityInStockQueryAsync(
                productId: command.ProductId,
                ct: ct
            );

        // Product is not found or product quantity from input
        // is greater than product quantity in stock.
        if (
            Equals(objA: foundProduct, objB: default)
            || command.Quantity > foundProduct.QuantityInStock
        )
        {
            return new() { StatusCode = GuestAddToCartResponseStatusCode.INPUT_VALIDATION_FAIL };
        }

        // get cart from session.
        var sessionModel = await _userSession.Value.GetAsync<List<CartItemEntity>>(
            key: CommonConstant.App.AppCartSessionKey,
            ct: ct
        );

        List<CartItemEntity> foundCartItems;

        // If cart is found in session.
        if (!Equals(objA: sessionModel, objB: AppSessionModel<List<CartItemEntity>>.NotFound))
        {
            foundCartItems = sessionModel.Value;

            // find cart item in cart.
            var foundCartItem = foundCartItems.FirstOrDefault(predicate: cartItem =>
                cartItem.ProductId.Equals(value: command.ProductId)
            );

            // Cart item is not found.
            if (Equals(objA: foundCartItem, objB: default))
            {
                // Add new cart item.
                foundCartItems.Add(
                    item: new() { ProductId = command.ProductId, Quantity = command.Quantity }
                );
            }
            // New quantity of cart item is greater than product quantity in stock.
            else if (foundCartItem.Quantity + command.Quantity > foundProduct.QuantityInStock)
            {
                return new()
                {
                    StatusCode = GuestAddToCartResponseStatusCode.INPUT_VALIDATION_FAIL
                };
            }
            // update cart item quantity.
            else
            {
                foundCartItem.Quantity += command.Quantity;
            }
        }
        // Init new cart with cart item if cart is not found in session.
        else
        {
            foundCartItems = new(capacity: 1)
            {
                new()
                {
                    ProductId = command.ProductId,
                    Quantity = command.Quantity,
                    UserId = CommonConstant.App.DefaultGuidValue
                }
            };
        }

        // set to session again
        await _userSession.Value.SetAsync(
            key: CommonConstant.App.AppCartSessionKey,
            value: foundCartItems,
            ct: ct
        );

        return new() { StatusCode = GuestAddToCartResponseStatusCode.OPERATION_SUCCESS };
    }
}
