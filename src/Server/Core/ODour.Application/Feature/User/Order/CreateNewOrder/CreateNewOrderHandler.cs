using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ODour.Application.Share.Common;
using ODour.Application.Share.Features;
using ODour.Domain.Feature.Main;
using ODour.Domain.Share.Order.Entities;

namespace ODour.Application.Feature.User.Order.CreateNewOrder;

internal sealed class CreateNewOrderHandler
    : IFeatureHandler<CreateNewOrderRequest, CreateNewOrderResponse>
{
    private readonly Lazy<IMainUnitOfWork> _mainUnitOfWork;

    public CreateNewOrderHandler(Lazy<IMainUnitOfWork> mainUnitOfWork)
    {
        _mainUnitOfWork = mainUnitOfWork;
    }

    public async Task<CreateNewOrderResponse> ExecuteAsync(
        CreateNewOrderRequest command,
        CancellationToken ct
    )
    {
        // Is payment method found.
        var isPaymentMethodFound =
            await _mainUnitOfWork.Value.CreateNewOrderRepository.IsPaymentMethodValidQueryAsync(
                paymentMethodId: command.PaymentMethodId,
                ct: ct
            );

        // payment method is not found.
        if (!isPaymentMethodFound)
        {
            return new() { StatusCode = CreateNewOrderResponseStatusCode.INPUT_VALIDATION_FAIL };
        }

        // Init order item entities.
        var orderItemEntities = new List<OrderItemEntity>();

        foreach (var orderItem in command.OrderItems)
        {
            orderItemEntities.Add(
                item: new()
                {
                    ProductId = orderItem.ProductId,
                    SellingQuantity = orderItem.Quantity,
                }
            );
        }

        // Are order items valid.
        var areOrderItemsValid =
            await _mainUnitOfWork.Value.CreateNewOrderRepository.AreOrderItemsValidQueryAsync(
                orderItemEntities: orderItemEntities,
                userId: command.GetUserId(),
                ct: ct
            );

        // Order items are not valid.
        if (!areOrderItemsValid)
        {
            return new() { StatusCode = CreateNewOrderResponseStatusCode.INPUT_VALIDATION_FAIL };
        }

        // Init new order.
        var newOrder = await InitNewOrderAsync(
            command: command,
            orderItems: orderItemEntities,
            ct: ct
        );

        // Add order to database.
        var dbResult = await _mainUnitOfWork.Value.CreateNewOrderRepository.AddOrderCommandAsync(
            newOrder: newOrder,
            ct: ct
        );

        // Cannot add order to database.
        if (!dbResult)
        {
            return new() { StatusCode = CreateNewOrderResponseStatusCode.OPERATION_FAIL };
        }

        return new()
        {
            StatusCode = CreateNewOrderResponseStatusCode.OPERATION_SUCCESS,
            OrderStatusId = newOrder.OrderStatusId
        };
    }

    private async Task<OrderEntity> InitNewOrderAsync(
        CreateNewOrderRequest command,
        IEnumerable<OrderItemEntity> orderItems,
        CancellationToken ct
    )
    {
        var PayWhenReceivingPaymentMethodId = Guid.Parse(
            input: "845e7be4-b3e3-4483-9fde-65694ee2d9b9"
        );

        var WaitForProcessingOrderStatusId = Guid.Parse(
            input: "f86e2f2a-5dcc-4546-a4de-ea297ee22dc5"
        );

        var WaitForPaymentOrderStatusId = Guid.Parse(input: "0e696741-97f6-444a-b265-025c8c394fc9");

        var orderStatusId = WaitForPaymentOrderStatusId;

        // if payment method is pay when receiving.
        if (command.PaymentMethodId == PayWhenReceivingPaymentMethodId)
        {
            // set order status to wait for processing.
            orderStatusId = WaitForProcessingOrderStatusId;
        }

        // Init new order id.
        var orderId = Guid.NewGuid();

        // Populate order items from cart items.
        await _mainUnitOfWork.Value.CreateNewOrderRepository.PopulateOrderItemQueryAsync(
            orderItems: orderItems,
            orderId: orderId,
            ct: ct
        );

        return new OrderEntity
        {
            Id = orderId,
            OrderStatusId = orderStatusId,
            FullName = command.FullName,
            PhoneNumber = command.PhoneNumber,
            UserId = command.GetUserId(),
            PaymentMethodId = command.PaymentMethodId,
            OrderCode = OrderEntity.GenerateOrderCode(dateTime: DateTime.UtcNow),
            OrderNote = command.OrderNote,
            TotalPrice = orderItems.Sum(selector: item => item.SellingPrice * item.SellingQuantity),
            DeliveredAddress = command.DeliveredAddress,
            DeliveredAt = CommonConstant.App.MinTimeInUTC,
            OrderItems = orderItems
        };
    }
}
