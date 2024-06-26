using System;
using System.Collections.Generic;
using ODour.Application.Share.Features;

namespace ODour.Application.Feature.User.Order.GetUserOrders;

public sealed class GetUserOrdersResponse : IFeatureResponse
{
    public GetUserOrdersResponseStatusCode StatusCode { get; init; }

    public ResponseBody Body { get; init; }

    public sealed class ResponseBody
    {
        public IEnumerable<Order> Orders { get; init; }

        public sealed class Order
        {
            public Guid Id { get; init; }

            public decimal Price { get; init; }

            public string Status { get; init; }

            public Item FirstProduct { get; init; }

            public sealed class Item
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
}
