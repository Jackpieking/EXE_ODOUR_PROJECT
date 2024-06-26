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

namespace ODour.Application.Feature.Guest.Cart.SyncGuestCartToUserCart;

internal sealed class SyncGuestCartToUserCartHandler
    : IFeatureHandler<SyncGuestCartToUserCartRequest, SyncGuestCartToUserCartResponse>
{
    private readonly Lazy<IMainUnitOfWork> _mainUnitOfWork;
    private readonly Lazy<IUserSession> _userSession;

    public SyncGuestCartToUserCartHandler(
        Lazy<IMainUnitOfWork> mainUnitOfWork,
        Lazy<IUserSession> userSession
    )
    {
        _mainUnitOfWork = mainUnitOfWork;
        _userSession = userSession;
    }

    public async Task<SyncGuestCartToUserCartResponse> ExecuteAsync(
        SyncGuestCartToUserCartRequest command,
        CancellationToken ct
    )
    {
        // Get guest session.
        var sessionModel = await _userSession.Value.GetAsync<List<CartItemEntity>>(
            key: CommonConstant.App.AppCartSessionKey,
            ct: ct
        );

        // No session is found or user cart is empty, then return successfully.
        if (
            Equals(sessionModel, AppSessionModel<List<CartItemEntity>>.NotFound)
            || sessionModel.Value.Count == default
        )
        {
            return new()
            {
                StatusCode = SyncGuestCartToUserCartResponseStatusCode.OPERATION_SUCCESS
            };
        }

        // Find all product info of user cart.
        var currentUserCartItems =
            await _mainUnitOfWork.Value.SyncGuestCartToUserCartRepository.FindCartByUserIdQueryAsync(
                userId: command.GetUserId(),
                ct: ct
            );

        var newUserCartItems = new List<CartItemEntity>();

        // Merge guest cart into user cart.
        foreach (var guestCartItem in sessionModel.Value)
        {
            // Find the product in guest cart.
            var currentUserCartItem = currentUserCartItems.FirstOrDefault(
                predicate: currentUserCartItem =>
                    currentUserCartItem.ProductId.Equals(value: guestCartItem.ProductId)
            );

            // If the product in guest cart found in user cart, update quantity.
            if (!Equals(objA: currentUserCartItem, objB: default))
            {
                var newUserCartItemQuantity = guestCartItem.Quantity + currentUserCartItem.Quantity;

                // Check if new user cart item quantity is greater than product quantity in stock.
                if (newUserCartItemQuantity > currentUserCartItem.Product.QuantityInStock)
                {
                    // Set new user cart item quantity to product quantity in stock.
                    currentUserCartItem.Quantity = currentUserCartItem.Product.QuantityInStock;
                }
                else
                {
                    // Set new user cart item quantity.
                    currentUserCartItem.Quantity = newUserCartItemQuantity;
                }
            }
            else
            {
                guestCartItem.UserId = command.GetUserId();

                newUserCartItems.Add(item: guestCartItem);
            }
        }

        // Update real user cart.
        var dbResult =
            await _mainUnitOfWork.Value.SyncGuestCartToUserCartRepository.UpdateUserCartCommandAsync(
                newUserCartItems: newUserCartItems,
                currentUserCartItems: currentUserCartItems,
                userId: command.GetUserId(),
                ct: ct
            );

        if (!dbResult)
        {
            return new() { StatusCode = SyncGuestCartToUserCartResponseStatusCode.OPERATION_FAIL };
        }

        // Remove everything in session
        await _userSession.Value.SetAsync<List<CartItemEntity>>(
            key: CommonConstant.App.AppCartSessionKey,
            value: new(capacity: default),
            ct: ct
        );

        return new() { StatusCode = SyncGuestCartToUserCartResponseStatusCode.OPERATION_SUCCESS };
    }
}
