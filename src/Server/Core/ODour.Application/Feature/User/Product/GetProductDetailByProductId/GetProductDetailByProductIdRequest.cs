using ODour.Application.Share.Features;

namespace ODour.Application.Feature.User.Product.GetProductDetailByProductId;

public sealed class GetProductDetailByProductIdRequest
    : IFeatureRequest<GetProductDetailByProductIdResponse>
{
    public string ProductId { get; set; }
}
