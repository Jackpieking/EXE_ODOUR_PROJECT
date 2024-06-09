namespace ODour.FastEndpointApi.Feature.User.Product.GetAllProducts.Common;

internal sealed class GetAllProductsStateBag
{
    public string CacheKey { get; set; }

    public int CacheDurationInSeconds { get; } = 60;
}
