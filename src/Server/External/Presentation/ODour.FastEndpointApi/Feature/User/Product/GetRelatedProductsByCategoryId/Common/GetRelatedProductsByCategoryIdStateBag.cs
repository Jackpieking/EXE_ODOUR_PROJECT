namespace ODour.FastEndpointApi.Feature.User.Product.GetRelatedProductsByCategoryId.Common;

internal sealed class GetRelatedProductsByCategoryIdStateBag
{
    public string CacheKey { get; set; }

    public int CacheDurationInSeconds { get; } = 15;
}
