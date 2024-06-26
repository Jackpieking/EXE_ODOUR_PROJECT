using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Feature.Guest.Cart.GetCartDetail;
using ODour.FastEndpointApi.Feature.Guest.Cart.GetCartDetail.Common;
using ODour.FastEndpointApi.Feature.Guest.Cart.GetCartDetail.HttpResponse;

namespace ODour.FastEndpointApi.Feature.Guest.Cart.GetCartDetail.Middlewares;

internal sealed class GuestGetCartDetailValidationPreProcessor
    : PreProcessor<EmptyRequest, GuestGetCartDetailStateBag>
{
    public override async Task PreProcessAsync(
        IPreProcessorContext<EmptyRequest> context,
        GuestGetCartDetailStateBag state,
        CancellationToken ct
    )
    {
        if (context.HasValidationFailures)
        {
            var httpResponse = GuestGetCartDetailHttpResponseManager
                .Resolve(statusCode: GuestGetCartDetailResponseStatusCode.INPUT_VALIDATION_FAIL)
                .Invoke(
                    arg1: state.AppRequest,
                    arg2: new()
                    {
                        StatusCode = GuestGetCartDetailResponseStatusCode.INPUT_VALIDATION_FAIL
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
