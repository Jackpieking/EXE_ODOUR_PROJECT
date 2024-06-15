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
            public long OrderCode { get; init; }

            public decimal OrderPrice { get; init; }

            public string OrderStatusName { get; init; }
        }
    }
}
