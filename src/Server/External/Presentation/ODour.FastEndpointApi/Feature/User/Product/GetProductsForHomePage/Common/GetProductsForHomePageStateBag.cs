using ODour.Application.Feature.User.Product.GetProductsForHomePage;

namespace ODour.FastEndpointApi.Feature.User.Product.GetProductsForHomePage.Common;

internal sealed class GetProductsForHomePageStateBag
{
    public GetProductsForHomePageRequest AppRequest { get; } = new();
}
