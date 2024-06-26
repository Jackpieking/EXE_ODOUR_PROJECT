using System;
using System.Collections.Generic;
using ODour.Application.Share.Features;

namespace ODour.Application.Feature.User.Order.GetOrderDetail;

public sealed class GetOrderDetailResponse : IFeatureResponse
{
    public GetOrderDetailResponseStatusCode StatusCode { get; init; }

    public ResponseBody Body { get; init; }

    public sealed class ResponseBody
    {
        public string OrderStatusName { get; init; }

        public string PaymentMethodName { get; init; }

        public string FullName { get; init; }

        public string DeliveredAddress { get; init; }

        public string PhoneNumber { get; init; }

        public string OrderNote { get; init; }

        public DateTime DeliveredAt { get; init; }

        public decimal TotalPrice { get; init; }

        public IEnumerable<Product> OrderItems { get; init; }

        public sealed class Product
        {
            public string Id { get; init; }

            public string Name { get; init; }

            public decimal SellingPrice { get; init; }

            public int SellingQuantity { get; init; }

            public decimal Total { get; init; }

            public string Image { get; init; }
        }
    }
}
