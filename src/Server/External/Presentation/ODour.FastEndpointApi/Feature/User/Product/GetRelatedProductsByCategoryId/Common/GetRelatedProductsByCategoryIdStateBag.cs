namespace ODour.FastEndpointApi.Feature.User.Product.GetRelatedProductsByCategoryId.Common;

internal sealed class GetRelatedProductsByCategoryIdStateBag
{
    internal string CacheKey { get; set; }

    internal int CacheDurationInSeconds { get; } = 15;
}
