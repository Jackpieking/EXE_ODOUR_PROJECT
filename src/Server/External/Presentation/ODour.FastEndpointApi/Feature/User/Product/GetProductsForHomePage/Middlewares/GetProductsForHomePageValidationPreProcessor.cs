using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Feature.User.Product.GetProductsForHomePage;
using ODour.FastEndpointApi.Feature.User.Product.GetProductsForHomePage.Common;
using ODour.FastEndpointApi.Feature.User.Product.GetProductsForHomePage.HttpResponse;

namespace ODour.FastEndpointApi.Feature.User.Product.GetProductsForHomePage.Middlewares;

internal sealed class GetProductsForHomePageValidationPreProcessor
    : PreProcessor<EmptyRequest, GetProductsForHomePageStateBag>
{
    public override async Task PreProcessAsync(
        IPreProcessorContext<EmptyRequest> context,
        GetProductsForHomePageStateBag state,
        CancellationToken ct
    )
    {
        if (context.HasValidationFailures)
        {
            var httpResponse = GetProductsForHomePageHttpResponseManager
                .Resolve(statusCode: GetProductsForHomePageResponseStatusCode.INPUT_VALIDATION_FAIL)
                .Invoke(
                    arg1: state.AppRequest,
                    arg2: new()
                    {
                        StatusCode = GetProductsForHomePageResponseStatusCode.INPUT_VALIDATION_FAIL
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
