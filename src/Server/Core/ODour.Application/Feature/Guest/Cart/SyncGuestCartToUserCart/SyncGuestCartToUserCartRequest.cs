using System;
using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Guest.Cart.SyncGuestCartToUserCart;

public sealed class SyncGuestCartToUserCartRequest
    : IFeatureRequest<SyncGuestCartToUserCartResponse>
{
    private Guid _userId;

    public Guid GetUserId() => _userId;

    public void SetUserId(Guid userId) => _userId = userId;
}
