using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ODour.Application.Share.Features;
using ODour.Domain.Feature.Main;

namespace ODour.Application.Feature.User.Cart.GetCartDetail;

internal sealed class GetCartDetailHandler
    : IFeatureHandler<GetCartDetailRequest, GetCartDetailResponse>
{
    private readonly Lazy<IMainUnitOfWork> _mainUnitOfWork;

    public GetCartDetailHandler(Lazy<IMainUnitOfWork> mainUnitOfWork)
    {
        _mainUnitOfWork = mainUnitOfWork;
    }

    public async Task<GetCartDetailResponse> ExecuteAsync(
        GetCartDetailRequest command,
        CancellationToken ct
    )
    {
        // Get cart items of the user from database.
        var foundCartItems =
            await _mainUnitOfWork.Value.GetCartDetailRepository.GetCartItemsOfUserQueryAsync(
                userId: command.GetUserId(),
                ct: ct
            );

        #region ProjectToResponse
        var foundProductsInCart = new List<GetCartDetailResponse.ResponseBody.Product>();

        decimal orderBill = default;

        foreach (var cartItem in foundCartItems)
        {
            var productTotalPrice = cartItem.Product.UnitPrice * cartItem.Quantity;

            orderBill += productTotalPrice;

            foundProductsInCart.Add(
                new()
                {
                    Id = cartItem.ProductId,
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
