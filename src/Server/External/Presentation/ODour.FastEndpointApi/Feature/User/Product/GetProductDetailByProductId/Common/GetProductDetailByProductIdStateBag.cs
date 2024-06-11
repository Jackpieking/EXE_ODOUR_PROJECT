namespace ODour.FastEndpointApi.Feature.User.Product.GetProductDetailByProductId.Common;

internal sealed class GetProductDetailByProductIdStateBag
{
    public string CacheKey { get; set; }

    public int CacheDurationInSeconds { get; } = 180;
}
