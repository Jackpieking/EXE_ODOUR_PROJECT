using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ODour.Application.Share.Features;
using ODour.Domain.Feature.Main;
using ODour.Domain.Share.Product.Entities;

namespace ODour.Application.Feature.User.Product.GetProductDetailByProductId;

internal sealed class GetProductDetailByProductIdHandler
    : IFeatureHandler<GetProductDetailByProductIdRequest, GetProductDetailByProductIdResponse>
{
    private readonly Lazy<IMainUnitOfWork> _unitOfWork;

    public GetProductDetailByProductIdHandler(Lazy<IMainUnitOfWork> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetProductDetailByProductIdResponse> ExecuteAsync(
        GetProductDetailByProductIdRequest command,
        CancellationToken ct
    )
    {
        // Is product found by product id.
        var isProductFound =
            await _unitOfWork.Value.GetProductDetailByProductIdRepository.IsProductFoundByProductIdQueryAsync(
                productId: command.ProductId,
                ct: ct
            );

        // Product not found.
        if (!isProductFound)
        {
            return new()
            {
                StatusCode = GetProductDetailByProductIdResponseStatusCode.PRODUCT_NOT_FOUND
            };
        }

        // Get product detail.
        var product =
            await _unitOfWork.Value.GetProductDetailByProductIdRepository.GetProductDetailByProductIdQueryAsync(
                productId: command.ProductId,
                ct: ct
            );

        return GenerateResponse(foundProduct: product);
    }

    private static GetProductDetailByProductIdResponse GenerateResponse(ProductEntity foundProduct)
    {
        return new()
        {
            StatusCode = GetProductDetailByProductIdResponseStatusCode.OPERATION_SUCCESS,
            Body = new()
            {
                Product = new()
                {
                    Id = foundProduct.Id,
                    Name = foundProduct.Name,
                    UnitPrice = foundProduct.UnitPrice,
                    Description = foundProduct.Description,
                    QuantityInStock = foundProduct.QuantityInStock,
                    ProductStatus = foundProduct.ProductStatus.Name,
                    Medias = foundProduct.ProductMedias.Select(
                        image => new GetProductDetailByProductIdResponse.ResponseBody.ProductEntity.ProductMedia
                        {
                            UploadOrder = image.UploadOrder,
                            StorageUrl = image.StorageUrl
                        }
                    ),
                }
            }
        };
    }
}
