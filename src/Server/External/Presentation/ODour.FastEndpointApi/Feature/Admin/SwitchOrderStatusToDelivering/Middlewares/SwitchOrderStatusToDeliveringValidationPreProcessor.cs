using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Feature.Admin.Order.SwitchOrderStatusToDelivering;
using ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToDelivering.Common;
using ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToDelivering.HttpResponse;

namespace ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToDelivering.Middlewares;

internal sealed class SwitchOrderStatusToDeliveringValidationPreProcessor
    : PreProcessor<SwitchOrderStatusToDeliveringRequest, SwitchOrderStatusToDeliveringStateBag>
{
    public override async Task PreProcessAsync(
        IPreProcessorContext<SwitchOrderStatusToDeliveringRequest> context,
        SwitchOrderStatusToDeliveringStateBag state,
        CancellationToken ct
    )
    {
        if (context.HasValidationFailures)
        {
            var httpResponse = SwitchOrderStatusToDeliveringHttpResponseManager
                .Resolve(
                    statusCode: SwitchOrderStatusToDeliveringResponseStatusCode.INPUT_VALIDATION_FAIL
                )
                .Invoke(
                    arg1: context.Request,
                    arg2: new()
                    {
                        StatusCode =
                            SwitchOrderStatusToDeliveringResponseStatusCode.INPUT_VALIDATION_FAIL
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
