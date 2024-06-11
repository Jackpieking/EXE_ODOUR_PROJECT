using System;
using ODour.Application.Share.Features;

namespace ODour.Application.Feature.User.Cart.AddToCart;

public sealed class AddToCartRequest : IFeatureRequest<AddToCartResponse>
{
    public string ProductId { get; set; }

    public int Quantity { get; init; }

    private Guid _userId;

    public void SetUserId(Guid userId)
    {
        _userId = userId;
    }

    public Guid GetUserId()
    {
        return _userId;
    }
}
