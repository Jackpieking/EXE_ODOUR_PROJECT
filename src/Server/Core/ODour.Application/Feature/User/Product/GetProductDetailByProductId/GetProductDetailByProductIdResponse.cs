using System;
using System.Collections.Generic;
using ODour.Application.Share.Features;

namespace ODour.Application.Feature.User.Product.GetProductDetailByProductId;

public sealed class GetProductDetailByProductIdResponse : IFeatureResponse
{
    public GetProductDetailByProductIdResponseStatusCode StatusCode { get; init; }

    public ResponseBody Body { get; init; }

    public sealed class ResponseBody
    {
        public ProductEntity Product { get; init; }

        public sealed class ProductEntity
        {
            public string Id { get; init; }

            public string Name { get; init; }

            public decimal UnitPrice { get; init; }

            public string Description { get; init; }

            public int QuantityInStock { get; init; }

            public string ProductStatus { get; init; }

            public ProductCategory Category { get; init; }

            public IEnumerable<ProductMedia> Medias { get; init; }

            public sealed class ProductMedia
            {
                public int UploadOrder { get; init; }

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
