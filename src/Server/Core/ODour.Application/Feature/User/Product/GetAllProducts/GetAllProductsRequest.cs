using System;
using ODour.Application.Share.Features;

namespace ODour.Application.Feature.User.Product.GetAllProducts;

public sealed class GetAllProductsRequest : IFeatureRequest<GetAllProductsResponse>
{
    internal int PageSize { get; } = 9;

    public int CurrentPage { get; set; }

    public string SortType { get; set; }

    public Guid CategoryId { get; init; }
}
