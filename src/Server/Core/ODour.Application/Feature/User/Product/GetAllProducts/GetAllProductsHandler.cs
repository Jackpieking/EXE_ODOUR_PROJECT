using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ODour.Application.Share.Features;
using ODour.Domain.Feature.Main;
using ODour.Domain.Share.Product.Entities;
using static ODour.Application.Share.Common.CommonConstant;

namespace ODour.Application.Feature.User.Product.GetAllProducts;

internal sealed class GetAllProductsHandler
    : IFeatureHandler<GetAllProductsRequest, GetAllProductsResponse>
{
    private readonly Lazy<IMainUnitOfWork> _unitOfWork;

    public GetAllProductsHandler(Lazy<IMainUnitOfWork> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetAllProductsResponse> ExecuteAsync(
        GetAllProductsRequest command,
        CancellationToken ct
    )
    {
        #region InputValidation
        // Validate category id.
        if (command.CategoryId != App.DefaultGuidValue)
        {
            var isCategoryFound =
                await _unitOfWork.Value.GetAllProductsRepository.IsCategoryFoundQueryAsync(
                    categoryId: command.CategoryId,
                    ct: ct
                );

            if (!isCategoryFound)
            {
                return new()
                {
                    StatusCode = GetAllProductsResponseStatusCode.INPUT_VALIDATION_FAIL
                };
            }
        }

        // Validate sort type.
        var isSortFound = _unitOfWork.Value.GetAllProductsRepository.IsSortFilterFoundQuery(
            sortType: command.SortType
        );

        if (!isSortFound)
        {
            return new() { StatusCode = GetAllProductsResponseStatusCode.INPUT_VALIDATION_FAIL };
        }

        // Get all products count.
        var totalProductCount =
            await _unitOfWork.Value.GetAllProductsRepository.GetProductsCountQueryAsync(
                command.CategoryId,
                ct: ct
            );

        // Parse to total number of pages.
        var numberOfPages = totalProductCount / command.PageSize;

        // if there are still some leftover products,
        // add one more page to the total number of pages.
        if (totalProductCount % command.PageSize != default)
        {
            numberOfPages += 1;
        }

        // Set current page to the smallest page.
        if (command.CurrentPage < 1)
        {
            command.CurrentPage = 1;
        }
        // Set current page to the largest page.
        else if (command.CurrentPage > numberOfPages)
        {
            command.CurrentPage = numberOfPages;
        }
        #endregion

        var products = await _unitOfWork.Value.GetAllProductsRepository.GetAllProductsQueryAsync(
            sortType: command.SortType,
            currentPage: command.CurrentPage,
            categoryId: command.CategoryId,
            pageSize: command.PageSize,
            ct: ct
        );

        return GenerateResponse(foundProducts: products, numberOfPages: numberOfPages);
    }

    private static GetAllProductsResponse GenerateResponse(
        IEnumerable<ProductEntity> foundProducts,
        int numberOfPages
    )
    {
        return new()
        {
            StatusCode = GetAllProductsResponseStatusCode.OPERATION_SUCCESS,
            Body = new()
            {
                NumberOfPage = numberOfPages,
                Products = foundProducts.Select(
                    selector: product => new GetAllProductsResponse.ResponseBody.Product
                    {
                        Id = product.Id,
                        Name = product.Name,
                        UnitPrice = product.UnitPrice,
                        ProductStatus = product.ProductStatus.Name,
                        Category = new() { Id = product.Category.Id, Name = product.Category.Name },
                        Medias = product.ProductMedias.Select(
                            selector: image => new GetAllProductsResponse.ResponseBody.Product.ProductMedia
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
