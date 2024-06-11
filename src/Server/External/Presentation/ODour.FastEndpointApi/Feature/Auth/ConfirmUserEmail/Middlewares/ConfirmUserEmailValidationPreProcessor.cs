using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Feature.Auth.ConfirmUserEmail;
using ODour.FastEndpointApi.Feature.Auth.ConfirmUserEmail.Common;
using ODour.FastEndpointApi.Feature.Auth.ConfirmUserEmail.HttpResponse;

namespace ODour.FastEndpointApi.Feature.Auth.ConfirmUserEmail.Middlewares;

internal sealed class ConfirmUserEmailValidationPreProcessor
    : PreProcessor<ConfirmUserEmailRequest, ConfirmUserEmailStateBag>
{
    public override async Task PreProcessAsync(
        IPreProcessorContext<ConfirmUserEmailRequest> context,
        ConfirmUserEmailStateBag state,
        CancellationToken ct
    )
    {
        if (context.HasValidationFailures)
        {
            var httpResponse = ConfirmUserEmailHttpResponseManager
                .Resolve(statusCode: ConfirmUserEmailResponseStatusCode.INPUT_VALIDATION_FAIL)
                .Invoke(
                    arg1: context.Request,
                    arg2: new()
                    {
                        StatusCode = ConfirmUserEmailResponseStatusCode.INPUT_VALIDATION_FAIL
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
