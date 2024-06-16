using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Feature.Admin.Order.SwitchOrderStatus;
using ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatus.Common;
using ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatus.HttpResponse;

namespace ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatus.Middlewares;

internal sealed class SwitchOrderStatusValidationPreProcessor
    : PreProcessor<SwitchOrderStatusRequest, SwitchOrderStatusStateBag>
{
    public override async Task PreProcessAsync(
        IPreProcessorContext<SwitchOrderStatusRequest> context,
        SwitchOrderStatusStateBag state,
        CancellationToken ct
    )
    {
        if (context.HasValidationFailures)
        {
            var httpResponse = SwitchOrderStatusHttpResponseManager
                .Resolve(statusCode: SwitchOrderStatusResponseStatusCode.INPUT_VALIDATION_FAIL)
                .Invoke(
                    arg1: context.Request,
                    arg2: new()
                    {
                        StatusCode = SwitchOrderStatusResponseStatusCode.INPUT_VALIDATION_FAIL
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
