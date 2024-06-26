using System;
using System.Collections.Generic;
using ODour.Application.Share.Features;

namespace ODour.Application.Feature.User.Order.CreateNewOrder;

public sealed class CreateNewOrderRequest : IFeatureRequest<CreateNewOrderResponse>
{
    public string FullName { get; init; }

    public string PhoneNumber { get; init; }

    public string DeliveredAddress { get; init; }

    public string OrderNote { get; init; }

    public Guid PaymentMethodId { get; init; }

    public IEnumerable<OrderItem> OrderItems { get; init; }

    private Guid _userId;

    public void SetUserId(Guid userId) => _userId = userId;

    public Guid GetUserId() => _userId;

    public sealed class OrderItem
    {
        public string ProductId { get; set; }

        public int Quantity { get; init; }
    }
}
