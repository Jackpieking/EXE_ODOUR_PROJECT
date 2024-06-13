using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Guest.Cart.RemoveFromCart;

public sealed class GuestRemoveFromCartResponse : IFeatureResponse
{
    public GuestRemoveFromCartResponseStatusCode StatusCode { get; init; }
}
