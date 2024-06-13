using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ODour.Application.Share.Common;
using ODour.Application.Share.Features;
using ODour.Application.Share.Session;
using ODour.Domain.Share.Cart.Entities;

namespace ODour.Application.Feature.Guest.Cart.RemoveFromCart;

internal sealed class GuestRemoveFromCartHandler
    : IFeatureHandler<GuestRemoveFromCartRequest, GuestRemoveFromCartResponse>
{
    private readonly Lazy<IUserSession> _userSession;

    public GuestRemoveFromCartHandler(Lazy<IUserSession> userSession)
    {
        _userSession = userSession;
    }

    public async Task<GuestRemoveFromCartResponse> ExecuteAsync(
        GuestRemoveFromCartRequest command,
        CancellationToken ct
    )
    {
        // validate input in database
        #region Validation
        // Find user cart.
        var sessionModel = await _userSession.Value.GetAsync<List<CartItemEntity>>(
            key: CommonConstant.App.AppCartSessionKey,
            ct: ct
        );

        // User cart not found.
        if (Equals(objA: sessionModel, objB: AppSessionModel<List<CartItemEntity>>.NotFound))
        {
            return new() { StatusCode = GuestRemoveFromCartResponseStatusCode.OPERATION_SUCCESS };
        }

        // Validate cart item input.
        var isInputValid = sessionModel.Value.Any(predicate: cartItem =>
            cartItem.ProductId.Equals(value: command.ProductId)
            && cartItem.Quantity - command.Quantity >= default(int)
        );

        if (!isInputValid)
        {
            return new()
            {
                StatusCode = GuestRemoveFromCartResponseStatusCode.INPUT_VALIDATION_FAIL
            };
        }
        #endregion

        // Find cart item.
        var foundCartItem = sessionModel.Value.First(predicate: cartItem =>
            cartItem.ProductId.Equals(value: command.ProductId)
        );

        // Update quantity again
        foundCartItem.Quantity -= command.Quantity;

        // Remove cart item if quantity is 0
        if (foundCartItem.Quantity == default)
        {
            sessionModel.Value.Remove(item: foundCartItem);
        }

        // Set cart to session
        await _userSession.Value.SetAsync(
            key: CommonConstant.App.AppCartSessionKey,
            value: sessionModel.Value,
            ct: ct
        );

        // return response
        return new() { StatusCode = GuestRemoveFromCartResponseStatusCode.OPERATION_SUCCESS };
    }
}
