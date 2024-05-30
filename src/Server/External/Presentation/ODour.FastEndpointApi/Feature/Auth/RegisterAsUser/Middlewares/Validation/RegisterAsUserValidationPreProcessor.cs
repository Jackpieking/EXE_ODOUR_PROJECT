using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Feature.Auth.RegisterAsUser;
using ODour.FastEndpointApi.Feature.Auth.RegisterAsUser.Common;
using ODour.FastEndpointApi.Feature.Auth.RegisterAsUser.HttpResponse;

namespace ODour.FastEndpointApi.Feature.Auth.RegisterAsUser.Middlewares.Validation;

internal sealed class RegisterAsUserValidationPreProcessor
    : PreProcessor<RegisterAsUserRequest, RegisterAsUserStateBag>
{
    public override async Task PreProcessAsync(
        IPreProcessorContext<RegisterAsUserRequest> context,
        RegisterAsUserStateBag state,
        CancellationToken ct
    )
    {
        if (context.HasValidationFailures)
        {
            var httpResponse = LazyRegisterAsUserHttResponseMapper
                .Get()
                .Resolve(statusCode: RegisterAsUserResponseStatusCode.INPUT_VALIDATION_FAIL)
                .Invoke(
                    arg1: context.Request,
                    arg2: new()
                    {
                        StatusCode = RegisterAsUserResponseStatusCode.INPUT_VALIDATION_FAIL
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
