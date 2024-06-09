using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ODour.Application.Share.Features;
using ODour.Domain.Feature.Main;
using ODour.Domain.Share.Product.Entities;

namespace ODour.Application.Feature.User.Product.GetProductsForHomePage;

internal sealed class GetProductsForHomePageHandler
    : IFeatureHandler<GetProductsForHomePageRequest, GetProductsForHomePageResponse>
{
    private readonly Lazy<IMainUnitOfWork> _mainUnitOfWork;

    public GetProductsForHomePageHandler(Lazy<IMainUnitOfWork> mainUnitOfWork)
    {
        _mainUnitOfWork = mainUnitOfWork;
    }

    public async Task<GetProductsForHomePageResponse> ExecuteAsync(
        GetProductsForHomePageRequest command,
        CancellationToken ct
    )
    {
        // Get all new products.
        var newProducts =
            await _mainUnitOfWork.Value.GetProductsForHomePageRepository.GetNewProductsQueryAsync(
                numberOfProducts: command.NumberOfNewProducts,
                ct
            );

        // Get all best selling products.
        var bestSellingProducts =
            await _mainUnitOfWork.Value.GetProductsForHomePageRepository.GetBestSellingProductsQueryAsync(
                numberOfProducts: command.NumberOfBestSellingProducts,
                ct
            );

        return GenerateResponse(newProducts: newProducts, bestSellingProducts: bestSellingProducts);
    }

    private static GetProductsForHomePageResponse GenerateResponse(
        IEnumerable<ProductEntity> newProducts,
        IEnumerable<ProductEntity> bestSellingProducts
    )
    {
        return new()
        {
            StatusCode = GetProductsForHomePageResponseStatusCode.OPERATION_SUCCESS,
            Body = new()
            {
                NewProducts = newProducts.Select(
                    selector: product => new GetProductsForHomePageResponse.ResponseBody.Product
                    {
                        Id = product.Id,
                        Name = product.Name,
                        UnitPrice = product.UnitPrice,
                        ProductStatus = product.ProductStatus.Name,
                        Category = new() { Id = product.Category.Id, Name = product.Category.Name },
                        Medias = product.ProductMedias.Select(
                            selector: image => new GetProductsForHomePageResponse.ResponseBody.Product.ProductMedia
                            {
                                StorageUrl = image.StorageUrl
                            }
                        )
                    }
                ),
                BestSellingProducts = bestSellingProducts.Select(
                    selector: product => new GetProductsForHomePageResponse.ResponseBody.Product
                    {
                        Id = product.Id,
                        Name = product.Name,
                        UnitPrice = product.UnitPrice,
                        ProductStatus = product.ProductStatus.Name,
                        Category = new() { Id = product.Category.Id, Name = product.Category.Name },
                        Medias = product.ProductMedias.Select(
                            selector: image => new GetProductsForHomePageResponse.ResponseBody.Product.ProductMedia
                            {
                                StorageUrl = image.StorageUrl
                            }
                        )
                    }
                )
            }
        };
    }
}
