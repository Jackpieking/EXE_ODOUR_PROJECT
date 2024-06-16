namespace ODour.FastEndpointApi.Feature.User.Order.GetOrderDetail.Common;

internal sealed class GetOrderDetailStateBag
{
    internal string CacheKey { get; set; }

    internal int CacheDurationInSeconds { get; } = 180;
}
