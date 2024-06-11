using System;
using System.Collections.Generic;
using ODour.Application.Share.Features;

namespace ODour.Application.Feature.User.Product.GetAllProducts;

public sealed class GetAllProductsResponse : IFeatureResponse
{
    public GetAllProductsResponseStatusCode StatusCode { get; init; }

    public ResponseBody Body { get; init; }

    public sealed class ResponseBody
    {
        public IEnumerable<Product> Products { get; init; }

        public long NumberOfPage { get; init; }

        public sealed class Product
        {
            public string Id { get; init; }

            public string Name { get; init; }

            public decimal UnitPrice { get; init; }

            public string ProductStatus { get; init; }

            public ProductCategory Category { get; init; }

            public IEnumerable<ProductMedia> Medias { get; init; }

            public sealed class ProductMedia
            {
                public string StorageUrl { get; init; }
            }

            public sealed class ProductCategory
            {
                public Guid Id { get; init; }

                public string Name { get; init; }
            }
        }
    }
}
