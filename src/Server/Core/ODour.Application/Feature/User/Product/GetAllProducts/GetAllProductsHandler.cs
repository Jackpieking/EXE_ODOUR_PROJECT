using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ODour.Application.Share.Features;
using ODour.Domain.Feature.Main;

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
        // Get all products count.
        var totalProductCount =
            await _unitOfWork.Value.GetAllProductsRepository.GetProductsCountQueryAsync(ct: ct);

        // Parse to total number of pages.
        var numberOfPages = totalProductCount / command.PageSize;

        // if there are still some leftover products,
        // add one more page to the total number of pages.
        if (totalProductCount % command.PageSize != default)
        {
            numberOfPages += 1;
        }

        // No page is found.
        if (command.CurrentPage > numberOfPages)
        {
            return new()
            {
                StatusCode = GetAllProductsResponseStatusCode.INPUT_VALIDATION_FAIL,
                Body = new()
            };
        }
        #endregion

        var products = await _unitOfWork.Value.GetAllProductsRepository.GetAllProductsQueryAsync(
            currentPage: command.CurrentPage,
            pageSize: command.PageSize,
            ct: ct
        );

        return new()
        {
            StatusCode = GetAllProductsResponseStatusCode.OPERATION_SUCCESS,
            Body = new()
            {
                MaxPage = numberOfPages,
                Products = products.Select(
                    product => new GetAllProductsResponse.ResponseBody.Product
                    {
                        Id = product.Id,
                        Name = product.Name,
                        UnitPrice = product.UnitPrice,
                        Description = product.Description,
                        QuantityInStock = product.QuantityInStock,
                        ProductStatus = product.ProductStatus.Name
                    }
                )
            }
        };
    }
}
