namespace ODour.FastEndpointApi.Feature.User.Product.GetProductDetailByProductId.Common;

internal sealed class GetProductDetailByProductIdStateBag
{
    internal string CacheKey { get; set; }

    internal int CacheDurationInSeconds { get; } = 180;
}
