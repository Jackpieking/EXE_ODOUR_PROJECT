using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Feature.Auth.RegisterAsAdmin;
using ODour.FastEndpointApi.Feature.Auth.RegisterAsAdmin.Common;
using ODour.FastEndpointApi.Feature.Auth.RegisterAsAdmin.HttpResponse;

namespace ODour.FastEndpointApi.Feature.Auth.RegisterAsAdmin.Middleware;

internal sealed class RegisterAsAdminValidationPreProcessor
    : PreProcessor<RegisterAsAdminRequest, RegisterAsAdminStateBag>
{
    public override async Task PreProcessAsync(
        IPreProcessorContext<RegisterAsAdminRequest> context,
        RegisterAsAdminStateBag state,
        CancellationToken ct
    )
    {
        if (context.HasValidationFailures)
        {
            var httpResponse = LazyRegisterAsAdminHttpResponseManager
                .Get()
                .Resolve(statusCode: RegisterAsAdminResponseStatusCode.INPUT_VALIDATION_FAIL)
                .Invoke(
                    arg1: context.Request,
                    arg2: new()
                    {
                        StatusCode = RegisterAsAdminResponseStatusCode.INPUT_VALIDATION_FAIL
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

            return;
        }
    }
}
