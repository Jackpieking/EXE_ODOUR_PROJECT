namespace ODour.FastEndpointApi.Feature.User.Product.GetAllProducts.Common;

internal sealed class GetAllProductsStateBag
{
    internal string CacheKey { get; set; }

    internal int CacheDurationInSeconds { get; } = 180;
}
