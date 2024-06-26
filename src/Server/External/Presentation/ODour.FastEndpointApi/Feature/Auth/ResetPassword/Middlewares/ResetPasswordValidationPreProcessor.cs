using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Feature.Auth.ResetPassword;
using ODour.FastEndpointApi.Feature.Auth.ResetPassword.Common;
using ODour.FastEndpointApi.Feature.Auth.ResetPassword.HttpResponse;

namespace ODour.FastEndpointApi.Feature.Auth.ResetPassword.Middlewares;

internal sealed class ResetPasswordValidationPreProcessor
    : PreProcessor<ResetPasswordRequest, ResetPasswordStateBag>
{
    public override async Task PreProcessAsync(
        IPreProcessorContext<ResetPasswordRequest> context,
        ResetPasswordStateBag state,
        CancellationToken ct
    )
    {
        if (context.HasValidationFailures)
        {
            var httpResponse = ResetPasswordHttpResponseManager
                .Resolve(statusCode: ResetPasswordResponseStatusCode.INPUT_VALIDATION_FAIL)
                .Invoke(
                    arg1: context.Request,
                    arg2: new()
                    {
                        StatusCode = ResetPasswordResponseStatusCode.INPUT_VALIDATION_FAIL
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
