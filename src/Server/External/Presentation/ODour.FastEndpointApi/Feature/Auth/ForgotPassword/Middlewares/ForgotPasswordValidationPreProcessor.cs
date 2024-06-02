using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Feature.Auth.ForgotPassword;
using ODour.FastEndpointApi.Feature.Auth.ForgotPassword.Common;
using ODour.FastEndpointApi.Feature.Auth.ForgotPassword.HttpResponse;

namespace ODour.FastEndpointApi.Feature.Auth.ForgotPassword.Middlewares;

internal sealed class ForgotPasswordValidationPreProcessor
    : PreProcessor<ForgotPasswordRequest, ForgotPasswordStateBag>
{
    public override async Task PreProcessAsync(
        IPreProcessorContext<ForgotPasswordRequest> context,
        ForgotPasswordStateBag state,
        CancellationToken ct
    )
    {
        if (context.HasValidationFailures)
        {
            var httpResponse = LazyForgotPasswordHttpResponseManager
                .Get()
                .Resolve(statusCode: ForgotPasswordResponseStatusCode.INPUT_VALIDATION_FAIL)
                .Invoke(
                    arg1: context.Request,
                    arg2: new()
                    {
                        StatusCode = ForgotPasswordResponseStatusCode.INPUT_VALIDATION_FAIL
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

            context.HttpContext.MarkResponseStart();
        }
    }
}
