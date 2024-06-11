using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Feature.User.Cart.GetCartDetail;
using ODour.FastEndpointApi.Feature.User.Cart.GetCartDetail.Common;
using ODour.FastEndpointApi.Feature.User.Cart.GetCartDetail.HttpResponse;

namespace ODour.FastEndpointApi.Feature.User.Cart.GetCartDetail.Middlewares;

internal sealed class GetCartDetailValidationPreProcessor
    : PreProcessor<EmptyRequest, GetCartDetailStateBag>
{
    public override async Task PreProcessAsync(
        IPreProcessorContext<EmptyRequest> context,
        GetCartDetailStateBag state,
        CancellationToken ct
    )
    {
        if (context.HasValidationFailures)
        {
            var httpResponse = GetCartDetailHttpResponseManager
                .Resolve(statusCode: GetCartDetailResponseStatusCode.INPUT_VALIDATION_FAIL)
                .Invoke(
                    arg1: state.AppRequest,
                    arg2: new()
                    {
                        StatusCode = GetCartDetailResponseStatusCode.INPUT_VALIDATION_FAIL
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
