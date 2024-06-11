using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Feature.User.Product.GetProductDetailByProductId;
using ODour.FastEndpointApi.Feature.User.Product.GetProductDetailByProductId.Common;
using ODour.FastEndpointApi.Feature.User.Product.GetProductDetailByProductId.HttpResponse;

namespace ODour.FastEndpointApi.Feature.User.Product.GetProductDetailByProductId.Middlewares;

internal sealed class GetProductDetailByProductIdValidationPreProcessor
    : PreProcessor<GetProductDetailByProductIdRequest, GetProductDetailByProductIdStateBag>
{
    public override async Task PreProcessAsync(
        IPreProcessorContext<GetProductDetailByProductIdRequest> context,
        GetProductDetailByProductIdStateBag state,
        CancellationToken ct
    )
    {
        if (context.HasValidationFailures)
        {
            var httpResponse = GetProductDetailByProductIdHttpResponseManager
                .Resolve(
                    statusCode: GetProductDetailByProductIdResponseStatusCode.INPUT_VALIDATION_FAIL
                )
                .Invoke(
                    arg1: context.Request,
                    arg2: new()
                    {
                        StatusCode =
                            GetProductDetailByProductIdResponseStatusCode.INPUT_VALIDATION_FAIL
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
