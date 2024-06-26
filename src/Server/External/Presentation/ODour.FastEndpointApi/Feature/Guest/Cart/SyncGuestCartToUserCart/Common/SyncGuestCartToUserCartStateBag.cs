using ODour.Application.Feature.Guest.Cart.SyncGuestCartToUserCart;

namespace ODour.FastEndpointApi.Feature.Guest.Cart.SyncGuestCartToUserCart.Common;

internal sealed class SyncGuestCartToUserCartStateBag
{
    internal SyncGuestCartToUserCartRequest AppRequest { get; set; } = new();
}
