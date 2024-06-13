using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Guest.Cart.RemoveFromCart;

public sealed class GuestRemoveFromCartRequest : IFeatureRequest<GuestRemoveFromCartResponse>
{
    public string ProductId { get; set; }

    public int Quantity { get; init; }
}
