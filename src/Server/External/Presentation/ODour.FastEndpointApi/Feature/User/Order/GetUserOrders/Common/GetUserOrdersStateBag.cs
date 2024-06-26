namespace ODour.FastEndpointApi.Feature.User.Order.GetUserOrders.Common;

internal sealed class GetUserOrdersStateBag
{
    internal string CacheKey { get; set; }

    internal int CacheDurationInSeconds { get; } = 180;
}
