using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Feature.User.Product.GetRelatedProductsByCategoryId;
using ODour.FastEndpointApi.Feature.User.Product.GetRelatedProductsByCategoryId.Common;
using ODour.FastEndpointApi.Feature.User.Product.GetRelatedProductsByCategoryId.HttpResponse;

namespace ODour.FastEndpointApi.Feature.User.Product.GetRelatedProductsByCategoryId.Middlewares;

internal sealed class GetRelatedProductsByCategoryIdValidationPreProcessor
    : PreProcessor<GetRelatedProductsByCategoryIdRequest, GetRelatedProductsByCategoryIdStateBag>
{
    public override async Task PreProcessAsync(
        IPreProcessorContext<GetRelatedProductsByCategoryIdRequest> context,
        GetRelatedProductsByCategoryIdStateBag state,
        CancellationToken ct
    )
    {
        if (context.HasValidationFailures)
        {
            var httpResponse = GetRelatedProductsByCategoryIdHttpResponseManager
                .Resolve(
                    statusCode: GetRelatedProductsByCategoryIdResponseStatusCode.INPUT_VALIDATION_FAIL
                )
                .Invoke(
                    arg1: context.Request,
                    arg2: new()
                    {
                        StatusCode =
                            GetRelatedProductsByCategoryIdResponseStatusCode.INPUT_VALIDATION_FAIL
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
