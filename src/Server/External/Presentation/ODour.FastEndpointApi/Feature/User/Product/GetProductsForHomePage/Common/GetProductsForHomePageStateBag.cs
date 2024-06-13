using ODour.Application.Feature.User.Product.GetProductsForHomePage;

namespace ODour.FastEndpointApi.Feature.User.Product.GetProductsForHomePage.Common;

internal sealed class GetProductsForHomePageStateBag
{
    internal GetProductsForHomePageRequest AppRequest { get; } = new();

    internal string CacheKey { get; set; }

    internal int CacheDurationInSeconds { get; } = 180;
}
