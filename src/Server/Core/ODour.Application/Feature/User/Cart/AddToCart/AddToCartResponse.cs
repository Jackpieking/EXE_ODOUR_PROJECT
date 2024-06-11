using ODour.Application.Share.Features;

namespace ODour.Application.Feature.User.Cart.AddToCart;

public sealed class AddToCartResponse : IFeatureResponse
{
    public AddToCartResponseStatusCode StatusCode { get; init; }
}
