using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ODour.Application.Share.Features;
using ODour.Domain.Feature.Main;

namespace ODour.Application.Feature.User.Order.GetOrderDetail;

internal sealed class GetOrderDetailHandler
    : IFeatureHandler<GetOrderDetailRequest, GetOrderDetailResponse>
{
    private readonly Lazy<IMainUnitOfWork> _mainUnitOfWork;

    public GetOrderDetailHandler(Lazy<IMainUnitOfWork> mainUnitOfWork)
    {
        _mainUnitOfWork = mainUnitOfWork;
    }

    public async Task<GetOrderDetailResponse> ExecuteAsync(
        GetOrderDetailRequest command,
        CancellationToken ct
    )
    {
        // Is order found.
        var order = await _mainUnitOfWork.Value.GetOrderDetailRepository.IsOrderFoundQueryAsync(
            orderId: command.OrderId,
            ct: ct
        );

        // Order is not found.
        if (!order)
        {
            return new GetOrderDetailResponse
            {
                StatusCode = GetOrderDetailResponseStatusCode.ORDER_NOT_FOUND
            };
        }

        // Get order detail.
        var orderDetail =
            await _mainUnitOfWork.Value.GetOrderDetailRepository.GetOrderDetailQueryAsync(
                orderId: command.OrderId,
                ct: ct
            );

        // Processing response.
        return new()
        {
            StatusCode = GetOrderDetailResponseStatusCode.OPERATION_SUCCESS,
            Body = new()
            {
                OrderStatusName = orderDetail.OrderStatus.Name,
                PaymentMethodName = orderDetail.PaymentMethod.Name,
                FullName = orderDetail.FullName,
                DeliveredAddress = orderDetail.DeliveredAddress,
                PhoneNumber = orderDetail.PhoneNumber,
                OrderNote = orderDetail.OrderNote,
                DeliveredAt = orderDetail.DeliveredAt,
                TotalPrice = orderDetail.TotalPrice,
                OrderItems = orderDetail.OrderItems.Select(
                    selector: orderItem => new GetOrderDetailResponse.ResponseBody.Product
                    {
                        Id = orderItem.ProductId,
                        Name = orderItem.Product.Name,
                        SellingPrice = orderItem.SellingPrice,
                        SellingQuantity = orderItem.SellingQuantity
                    }
                )
            }
        };
    }
}
