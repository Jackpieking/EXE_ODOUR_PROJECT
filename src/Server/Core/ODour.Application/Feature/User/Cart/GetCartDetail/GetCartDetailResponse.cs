using System.Collections.Generic;
using ODour.Application.Share.Features;

namespace ODour.Application.Feature.User.Cart.GetCartDetail;

public sealed class GetCartDetailResponse : IFeatureResponse
{
    public GetCartDetailResponseStatusCode StatusCode { get; init; }

    public ResponseBody Body { get; init; }

    public sealed class ResponseBody
    {
        public decimal CurrentOrderPrice { get; set; }

        public IEnumerable<Product> OrderItems { get; set; }

        public sealed class Product
        {
            public string Id { get; set; }

            public string Name { get; set; }

            public string FirstImage { get; set; }

            public decimal UnitPrice { get; set; }

            public int Quantity { get; set; }

            public decimal TotalPrice { get; set; }
        }
    }
}
