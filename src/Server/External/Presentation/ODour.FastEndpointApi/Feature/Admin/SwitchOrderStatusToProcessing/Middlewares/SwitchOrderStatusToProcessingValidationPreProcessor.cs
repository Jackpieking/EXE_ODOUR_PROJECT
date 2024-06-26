using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Feature.Admin.Order.SwitchOrderStatusToProcessing;
using ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToProcessing.Common;
using ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToProcessing.HttpResponse;

namespace ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToProcessing.Middlewares;

internal sealed class SwitchOrderStatusToProcessingValidationPreProcessor
    : PreProcessor<SwitchOrderStatusToProcessingRequest, SwitchOrderStatusToProcessingStateBag>
{
    public override async Task PreProcessAsync(
        IPreProcessorContext<SwitchOrderStatusToProcessingRequest> context,
        SwitchOrderStatusToProcessingStateBag state,
        CancellationToken ct
    )
    {
        if (context.HasValidationFailures)
        {
            var httpResponse = SwitchOrderStatusToProcessingHttpResponseManager
                .Resolve(
                    statusCode: SwitchOrderStatusToProcessingResponseStatusCode.INPUT_VALIDATION_FAIL
                )
                .Invoke(
                    arg1: context.Request,
                    arg2: new()
                    {
                        StatusCode =
                            SwitchOrderStatusToProcessingResponseStatusCode.INPUT_VALIDATION_FAIL
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
