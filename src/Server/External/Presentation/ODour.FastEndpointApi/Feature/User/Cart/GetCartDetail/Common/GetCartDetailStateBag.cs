using ODour.Application.Feature.User.Cart.GetCartDetail;

namespace ODour.FastEndpointApi.Feature.User.Cart.GetCartDetail.Common;

internal sealed class GetCartDetailStateBag
{
    internal GetCartDetailRequest AppRequest { get; set; } = new();

    internal string CacheKey { get; set; }

    internal int CacheDurationInSeconds { get; } = 180;
}
