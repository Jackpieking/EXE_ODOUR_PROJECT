using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Guest.Cart.AddToCart;

public sealed class GuestAddToCartRequest : IFeatureRequest<GuestAddToCartResponse>
{
    public string ProductId { get; set; }

    public int Quantity { get; init; }
}
