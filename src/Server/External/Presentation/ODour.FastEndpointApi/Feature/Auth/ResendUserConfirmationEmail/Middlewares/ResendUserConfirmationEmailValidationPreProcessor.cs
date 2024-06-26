using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Feature.Auth.ResendUserConfirmationEmail;
using ODour.FastEndpointApi.Feature.Auth.ResendUserConfirmationEmail.Common;
using ODour.FastEndpointApi.Feature.Auth.ResendUserConfirmationEmail.HttpResponse;

namespace ODour.FastEndpointApi.Feature.Auth.ResendUserConfirmationEmail.Middlewares;

internal sealed class ResendUserConfirmationEmailValidationPreProcessor
    : PreProcessor<ResendUserConfirmationEmailRequest, ResendUserConfirmationEmailStateBag>
{
    public override async Task PreProcessAsync(
        IPreProcessorContext<ResendUserConfirmationEmailRequest> context,
        ResendUserConfirmationEmailStateBag state,
        CancellationToken ct
    )
    {
        if (context.HasValidationFailures)
        {
            var httpResponse = ResendUserConfirmationEmailHttpResponseManager
                .Resolve(
                    statusCode: ResendUserConfirmationEmailResponseStatusCode.INPUT_VALIDATION_FAIL
                )
                .Invoke(
                    arg1: context.Request,
                    arg2: new()
                    {
                        StatusCode =
                            ResendUserConfirmationEmailResponseStatusCode.INPUT_VALIDATION_FAIL
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
