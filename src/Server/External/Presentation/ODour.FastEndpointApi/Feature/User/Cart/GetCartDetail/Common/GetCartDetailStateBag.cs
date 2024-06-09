using ODour.Application.Feature.User.Cart.GetCartDetail;

namespace ODour.FastEndpointApi.Feature.User.Cart.GetCartDetail.Common;

internal sealed class GetCartDetailStateBag
{
    public GetCartDetailRequest AppRequest { get; set; } = new();
}
