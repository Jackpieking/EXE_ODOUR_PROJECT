using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Feature.Admin.Order.SwitchOrderStatusToCancelling;
using ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToCancelling.Common;
using ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToCancelling.HttpResponse;

namespace ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToCancelling.Middlewares;

internal sealed class SwitchOrderStatusToCancellingValidationPreProcessor
    : PreProcessor<SwitchOrderStatusToCancellingRequest, SwitchOrderStatusToCancellingStateBag>
{
    public override async Task PreProcessAsync(
        IPreProcessorContext<SwitchOrderStatusToCancellingRequest> context,
        SwitchOrderStatusToCancellingStateBag state,
        CancellationToken ct
    )
    {
        if (context.HasValidationFailures)
        {
            var httpResponse = SwitchOrderStatusToCancellingHttpResponseManager
                .Resolve(
                    statusCode: SwitchOrderStatusToCancellingResponseStatusCode.INPUT_VALIDATION_FAIL
                )
                .Invoke(
                    arg1: context.Request,
                    arg2: new()
                    {
                        StatusCode =
                            SwitchOrderStatusToCancellingResponseStatusCode.INPUT_VALIDATION_FAIL
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
