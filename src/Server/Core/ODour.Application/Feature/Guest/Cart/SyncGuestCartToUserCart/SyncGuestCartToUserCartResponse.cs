using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Guest.Cart.SyncGuestCartToUserCart;

public sealed class SyncGuestCartToUserCartResponse : IFeatureResponse
{
    public SyncGuestCartToUserCartResponseStatusCode StatusCode { get; init; }
}
