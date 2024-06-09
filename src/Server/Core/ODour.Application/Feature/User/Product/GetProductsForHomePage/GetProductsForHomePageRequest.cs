using ODour.Application.Share.Features;

namespace ODour.Application.Feature.User.Product.GetProductsForHomePage;

public sealed class GetProductsForHomePageRequest : IFeatureRequest<GetProductsForHomePageResponse>
{
    internal int NumberOfNewProducts { get; } = 10;

    internal int NumberOfBestSellingProducts { get; } = 10;
}
