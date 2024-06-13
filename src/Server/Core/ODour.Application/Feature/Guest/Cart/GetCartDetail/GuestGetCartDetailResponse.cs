using System.Collections.Generic;
using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Guest.Cart.GetCartDetail;

public sealed class GuestGetCartDetailResponse : IFeatureResponse
{
    public GuestGetCartDetailResponseStatusCode StatusCode { get; init; }

    public ResponseBody Body { get; init; }

    public sealed class ResponseBody
    {
        public decimal CurrentOrderPrice { get; init; }

        public IEnumerable<Product> OrderItems { get; init; }

        public sealed class Product
        {
            public string Id { get; init; }

            public string Name { get; init; }

            public string FirstImage { get; init; }

            public decimal UnitPrice { get; init; }

            public int Quantity { get; init; }

            public decimal TotalPrice { get; init; }
        }
    }
}
