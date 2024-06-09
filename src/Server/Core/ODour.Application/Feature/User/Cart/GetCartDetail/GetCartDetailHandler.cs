using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ODour.Application.Share.Common;
using ODour.Application.Share.Features;
using ODour.Application.Share.Session;
using ODour.Domain.Share.Cart.Entities;

namespace ODour.Application.Feature.User.Cart.GetCartDetail;

internal sealed class GetCartDetailHandler
    : IFeatureHandler<GetCartDetailRequest, GetCartDetailResponse>
{
    private readonly Lazy<IUserSession> _userSession;

    public GetCartDetailHandler(Lazy<IUserSession> userSession)
    {
        _userSession = userSession;
    }

    public async Task<GetCartDetailResponse> ExecuteAsync(
        GetCartDetailRequest command,
        CancellationToken ct
    )
    {
        var session = await _userSession.Value.GetAsync<List<CartItemEntity>>(
            key: CommonConstant.App.AppCartSessionKey,
            ct: ct
        );

        if (Equals(objA: session, objB: AppSessionModel<List<CartItemEntity>>.NotFound))
        {
            await _userSession.Value.AddAsync<List<CartItemEntity>>(
                key: $"{command.GetUserId()}{CommonConstant.App.DefaultStringSeparator}{CommonConstant.App.AppCartSessionKey}",
                value: new(capacity: default),
                ct: ct
            );

            return new()
            {
                StatusCode = GetCartDetailResponseStatusCode.OPERATION_SUCCESS,
                Body = new()
            };
        }

        #region ProjectToResponse
        var foundProductsInCart = new List<GetCartDetailResponse.ResponseBody.Product>();

        decimal orderBill = default;

        foreach (var cartItem in session.Value)
        {
            var productTotalPrice = cartItem.Product.UnitPrice * cartItem.Quantity;

            orderBill += productTotalPrice;

            foundProductsInCart.Add(
                new()
                {
                    Id = cartItem.Product.Id,
                    Name = cartItem.Product.Name,
                    FirstImage = cartItem.Product.ProductMedias.First().StorageUrl,
                    UnitPrice = cartItem.Product.UnitPrice,
                    Quantity = cartItem.Quantity,
                    TotalPrice = productTotalPrice
                }
            );
        }
        #endregion

        return new()
        {
            StatusCode = GetCartDetailResponseStatusCode.OPERATION_SUCCESS,
            Body = new() { CurrentOrderPrice = orderBill, OrderItems = foundProductsInCart }
        };
    }
}
