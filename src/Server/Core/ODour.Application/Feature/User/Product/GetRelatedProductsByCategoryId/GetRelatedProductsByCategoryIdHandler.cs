using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ODour.Application.Share.Features;
using ODour.Domain.Feature.Main;
using ODour.Domain.Share.Product.Entities;

namespace ODour.Application.Feature.User.Product.GetRelatedProductsByCategoryId;

internal sealed class GetRelatedProductsByCategoryIdHandler
    : IFeatureHandler<GetRelatedProductsByCategoryIdRequest, GetRelatedProductsByCategoryIdResponse>
{
    private readonly Lazy<IMainUnitOfWork> _mainUnitOfWork;

    public GetRelatedProductsByCategoryIdHandler(Lazy<IMainUnitOfWork> mainUnitOfWork)
    {
        _mainUnitOfWork = mainUnitOfWork;
    }

    public async Task<GetRelatedProductsByCategoryIdResponse> ExecuteAsync(
        GetRelatedProductsByCategoryIdRequest command,
        CancellationToken ct
    )
    {
        // Is category foudn by category id.
        var isCategoryFound =
            await _mainUnitOfWork.Value.GetRelatedProductsByCategoryIdRepository.IsCategoryFoundByCategoryIdQueryAsync(
                categoryId: command.CategoryId,
                ct
            );

        // Category is not found.
        if (!isCategoryFound)
        {
            return new()
            {
                StatusCode = GetRelatedProductsByCategoryIdResponseStatusCode.CATEGORY_NOT_FOUND
            };
        }

        // Get number of related products by category id.
        var relatedProducts =
            await _mainUnitOfWork.Value.GetRelatedProductsByCategoryIdRepository.GetRelatedProductsByCategoryIdQueryAsync(
                categoryId: command.CategoryId,
                numberOfProducts: command.NumberOfRelatedProducts,
                ct
            );

        return GenerateResponse(foundProducts: relatedProducts);
    }

    private static GetRelatedProductsByCategoryIdResponse GenerateResponse(
        IEnumerable<ProductEntity> foundProducts
    )
    {
        return new()
        {
            StatusCode = GetRelatedProductsByCategoryIdResponseStatusCode.OPERATION_SUCCESS,
            Body = new()
            {
                Products = foundProducts.Select(
                    selector: product => new GetRelatedProductsByCategoryIdResponse.ResponseBody.Product
                    {
                        Id = product.Id,
                        Name = product.Name,
                        UnitPrice = product.UnitPrice,
                        ProductStatus = product.ProductStatus.Name,
                        Category = new() { Id = product.Category.Id, Name = product.Category.Name },
                        Medias = product.ProductMedias.Select(
                            selector: image => new GetRelatedProductsByCategoryIdResponse.ResponseBody.Product.ProductMedia
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
