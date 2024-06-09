using System;
using ODour.Application.Share.Features;

namespace ODour.Application.Feature.User.Cart.GetCartDetail;

public sealed class GetCartDetailRequest : IFeatureRequest<GetCartDetailResponse>
{
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
