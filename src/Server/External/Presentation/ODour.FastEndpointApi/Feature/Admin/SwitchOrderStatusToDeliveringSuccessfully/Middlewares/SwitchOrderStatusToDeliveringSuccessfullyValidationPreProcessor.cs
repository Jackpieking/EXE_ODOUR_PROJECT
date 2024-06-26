using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Feature.Admin.Order.SwitchOrderStatusToDeliveringSuccessfully;
using ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToDeliveringSuccessfully.Common;
using ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToDeliveringSuccessfully.HttpResponse;

namespace ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToDeliveringSuccessfully.Middlewares;

internal sealed class SwitchOrderStatusToDeliveringSuccessfullyValidationPreProcessor
    : PreProcessor<
        SwitchOrderStatusToDeliveringSuccessfullyRequest,
        SwitchOrderStatusToDeliveringSuccessfullyStateBag
    >
{
    public override async Task PreProcessAsync(
        IPreProcessorContext<SwitchOrderStatusToDeliveringSuccessfullyRequest> context,
        SwitchOrderStatusToDeliveringSuccessfullyStateBag state,
        CancellationToken ct
    )
    {
        if (context.HasValidationFailures)
        {
            var httpResponse = SwitchOrderStatusToDeliveringSuccessfullyHttpResponseManager
                .Resolve(
                    statusCode: SwitchOrderStatusToDeliveringSuccessfullyResponseStatusCode.INPUT_VALIDATION_FAIL
                )
                .Invoke(
                    arg1: context.Request,
                    arg2: new()
                    {
                        StatusCode =
                            SwitchOrderStatusToDeliveringSuccessfullyResponseStatusCode.INPUT_VALIDATION_FAIL
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
