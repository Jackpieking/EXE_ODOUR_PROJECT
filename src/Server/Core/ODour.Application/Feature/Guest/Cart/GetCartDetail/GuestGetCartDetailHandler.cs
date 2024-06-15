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

namespace ODour.Application.Feature.Guest.Cart.GetCartDetail;

internal sealed class GuestGetCartDetailHandler
    : IFeatureHandler<GuestGetCartDetailRequest, GuestGetCartDetailResponse>
{
    private readonly Lazy<IUserSession> _userSession;
    private readonly Lazy<IMainUnitOfWork> _mainUnitOfWork;

    public GuestGetCartDetailHandler(
        Lazy<IUserSession> userSession,
        Lazy<IMainUnitOfWork> mainUnitOfWork
    )
    {
        _userSession = userSession;
        _mainUnitOfWork = mainUnitOfWork;
    }

    public async Task<GuestGetCartDetailResponse> ExecuteAsync(
        GuestGetCartDetailRequest command,
        CancellationToken ct
    )
    {
        // get cart from session.
        var sessionModel = await _userSession.Value.GetAsync<List<CartItemEntity>>(
            key: CommonConstant.App.AppCartSessionKey,
            ct: ct
        );

        // Cart is not found.
        if (Equals(objA: sessionModel, objB: AppSessionModel<List<CartItemEntity>>.NotFound))
        {
            // return empty cart.
            return new()
            {
                StatusCode = GuestGetCartDetailResponseStatusCode.OPERATION_SUCCESS,
                Body = new()
                {
                    OrderItems = Enumerable.Empty<GuestGetCartDetailResponse.ResponseBody.Product>()
                }
            };
        }

        // Populate all product details.
        var foundProducts =
            await _mainUnitOfWork.Value.GuestGetCartDetailRepository.PopulateAllProductDetailOfCartQueryAsync(
                productIds: sessionModel.Value.Select(selector: cartItem => cartItem.ProductId),
                ct: ct
            );

        #region ProjectToResponse
        var foundProductsInCart = new List<GuestGetCartDetailResponse.ResponseBody.Product>();

        decimal orderBill = default;

        foreach (var foundProduct in foundProducts)
        {
            // get cart item with the same product id.
            var cartItemWithTheSameProductId = sessionModel.Value.First(predicate: cartItem =>
                cartItem.ProductId.Equals(value: foundProduct.Id)
            );

            // calculate product total price.
            var productTotalPrice = foundProduct.UnitPrice * cartItemWithTheSameProductId.Quantity;

            // update order bill.
            orderBill += productTotalPrice;

            // add product to response.
            foundProductsInCart.Add(
                new()
                {
                    Id = foundProduct.Id,
                    Name = foundProduct.Name,
                    UnitPrice = foundProduct.UnitPrice,
                    Quantity = cartItemWithTheSameProductId.Quantity,
                    TotalPrice = productTotalPrice
                }
            );
        }
        #endregion

        return new()
        {
            StatusCode = GuestGetCartDetailResponseStatusCode.OPERATION_SUCCESS,
            Body = new() { OrderItems = foundProductsInCart, CurrentOrderPrice = orderBill }
        };
    }
}
