using ODour.Application.Feature.User.Cart.GetCartDetail;

namespace ODour.FastEndpointApi.Feature.User.Cart.GetCartDetail.Common;

internal sealed class GetCartDetailStateBag
{
    public GetCartDetailRequest AppRequest { get; set; } = new();

    public string CacheKey { get; set; }

    public int CacheDurationInSeconds { get; } = 180;
}
