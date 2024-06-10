using ODour.Application.Share.Features;

namespace ODour.Application.Feature.User.Cart.RemoveFromCart;

public sealed class RemoveFromCartResponse : IFeatureResponse
{
    public RemoveFromCartResponseStatusCode StatusCode { get; init; }
}
