using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Guest.Cart.AddToCart;

public sealed class GuestAddToCartResponse : IFeatureResponse
{
    public GuestAddToCartResponseStatusCode StatusCode { get; set; }
}
