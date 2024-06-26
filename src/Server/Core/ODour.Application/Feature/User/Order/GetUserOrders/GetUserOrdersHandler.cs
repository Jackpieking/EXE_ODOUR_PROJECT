using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ODour.Application.Share.Features;
using ODour.Domain.Feature.Main;

namespace ODour.Application.Feature.User.Order.GetUserOrders;

internal sealed class GetUserOrdersHandler
    : IFeatureHandler<GetUserOrdersRequest, GetUserOrdersResponse>
{
    private readonly Lazy<IMainUnitOfWork> _mainUnitOfWork;

    public GetUserOrdersHandler(Lazy<IMainUnitOfWork> mainUnitOfWork)
    {
        _mainUnitOfWork = mainUnitOfWork;
    }

    public async Task<GetUserOrdersResponse> ExecuteAsync(
        GetUserOrdersRequest command,
        CancellationToken ct
    )
    {
        // Is order status found.
        var isOrderStatusFound =
            await _mainUnitOfWork.Value.GetUserOrdersRepository.IsOrderStatusFoundQueryAsync(
                command.OrderStatusId,
                ct
            );

        if (!isOrderStatusFound)
        {
            return new GetUserOrdersResponse
            {
                StatusCode = GetUserOrdersResponseStatusCode.INPUT_VALIDATION_FAIL
            };
        }

        // Get user orders.
        var userOrders = await _mainUnitOfWork.Value.GetUserOrdersRepository.GetAllOrderQueryAsync(
            userId: command.GetUserId(),
            orderStatusId: command.OrderStatusId,
            ct: ct
        );

        return new()
        {
            StatusCode = GetUserOrdersResponseStatusCode.OPERATION_SUCCESS,
            Body = new()
            {
                Orders = userOrders.Select(
                    selector: order => new GetUserOrdersResponse.ResponseBody.Order
                    {
                        Id = order.Id,
                        Status = order.OrderStatus.Name,
                        Price = order.TotalPrice,
                        FirstProduct = order
                            .OrderItems.Select(
                                orderItem => new GetUserOrdersResponse.ResponseBody.Order.Item
                                {
                                    Id = orderItem.ProductId,
                                    Name = orderItem.Product.Name,
                                    SellingPrice = orderItem.SellingPrice,
                                    SellingQuantity = orderItem.SellingQuantity,
                                    Total = orderItem.SellingPrice * orderItem.SellingQuantity,
                                    Image =
                                        orderItem.Product.ProductMedias.FirstOrDefault().StorageUrl
                                        ?? string.Empty
                                }
                            )
                            .FirstOrDefault()
                    }
                )
            }
        };
    }
}
