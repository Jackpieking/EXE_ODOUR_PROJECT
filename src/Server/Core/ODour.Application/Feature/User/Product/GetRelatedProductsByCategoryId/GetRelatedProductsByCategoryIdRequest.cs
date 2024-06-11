using System;
using ODour.Application.Share.Features;

namespace ODour.Application.Feature.User.Product.GetRelatedProductsByCategoryId;

public sealed class GetRelatedProductsByCategoryIdRequest
    : IFeatureRequest<GetRelatedProductsByCategoryIdResponse>
{
    public Guid CategoryId { get; set; }

    internal int NumberOfRelatedProducts { get; } = 6;
}
