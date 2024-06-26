using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Feature.User.Product.GetAllProducts;
using ODour.FastEndpointApi.Feature.User.Product.GetAllProducts.Common;
using ODour.FastEndpointApi.Feature.User.Product.GetAllProducts.HttpResponse;

namespace ODour.FastEndpointApi.Feature.User.Product.GetAllProducts.Middlewares;

internal sealed class GetAllProductsValidationPreProcessor
    : PreProcessor<GetAllProductsRequest, GetAllProductsStateBag>
{
    public override async Task PreProcessAsync(
        IPreProcessorContext<GetAllProductsRequest> context,
        GetAllProductsStateBag state,
        CancellationToken ct
    )
    {
        if (context.HasValidationFailures)
        {
            var httpResponse = GetAllProductsHttpResponseManager
                .Resolve(statusCode: GetAllProductsResponseStatusCode.INPUT_VALIDATION_FAIL)
                .Invoke(
                    arg1: context.Request,
                    arg2: new()
                    {
                        StatusCode = GetAllProductsResponseStatusCode.INPUT_VALIDATION_FAIL
                    }
                );

            // Save http code temporarily and set http code of response to default for not serializing.
            var httpCode = httpResponse.HttpCode;
            httpResponse.HttpCode = default;

            await context.HttpContext.Response.SendAsync(
                response: httpResponse,
                statusCode: httpCode,
                cancellation: ct
            );

            return;
        }
    }
}
