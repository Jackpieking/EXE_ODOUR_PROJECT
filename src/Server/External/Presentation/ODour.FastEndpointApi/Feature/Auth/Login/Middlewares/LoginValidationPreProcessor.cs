using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Feature.Auth.Login;
using ODour.FastEndpointApi.Feature.Auth.Login.Common;
using ODour.FastEndpointApi.Feature.Auth.Login.HttpResponse;

namespace ODour.FastEndpointApi.Feature.Auth.Login.Middlewares;

internal sealed class LoginValidationPreProcessor : PreProcessor<LoginRequest, LoginStateBag>
{
    public override async Task PreProcessAsync(
        IPreProcessorContext<LoginRequest> context,
        LoginStateBag state,
        CancellationToken ct
    )
    {
        if (context.HasValidationFailures)
        {
            var httpResponse = LazyLoginHttpResponseManager
                .Get()
                .Resolve(statusCode: LoginResponseStatusCode.INPUT_VALIDATION_FAIL)
                .Invoke(
                    arg1: context.Request,
                    arg2: new() { StatusCode = LoginResponseStatusCode.INPUT_VALIDATION_FAIL }
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

            return;
        }
    }
}
