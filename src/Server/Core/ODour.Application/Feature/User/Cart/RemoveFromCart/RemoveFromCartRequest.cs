using System;
using ODour.Application.Share.Features;

namespace ODour.Application.Feature.User.Cart.RemoveFromCart;

public sealed class RemoveFromCartRequest : IFeatureRequest<RemoveFromCartResponse>
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
