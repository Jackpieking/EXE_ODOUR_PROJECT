using ODour.Application.Feature.Guest.Cart.GetCartDetail;

namespace ODour.FastEndpointApi.Feature.Guest.Cart.GetCartDetail.Common;

internal sealed class GuestGetCartDetailStateBag
{
    internal GuestGetCartDetailRequest AppRequest { get; set; } = new();

    internal string CacheKey { get; set; }

    internal int CacheDurationInSeconds { get; } = 180;
}
