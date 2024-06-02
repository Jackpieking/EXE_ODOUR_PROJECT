using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Feature.Auth.Logout;
using ODour.FastEndpointApi.Feature.Auth.Logout.Common;
using ODour.FastEndpointApi.Feature.Auth.Logout.HttpResponse;

namespace ODour.FastEndpointApi.Feature.Auth.Logout.Middlewares;

internal sealed class LogoutValidationPreProcessor : PreProcessor<LogoutRequest, LogoutStateBag>
{
    public override async Task PreProcessAsync(
        IPreProcessorContext<LogoutRequest> context,
        LogoutStateBag state,
        CancellationToken ct
    )
    {
        if (context.HasValidationFailures)
        {
            var httpResponse = LazyLogoutHttpResponseManager
                .Get()
                .Resolve(statusCode: LogoutResponseStatusCode.INPUT_VALIDATION_FAIL)
                .Invoke(
                    arg1: context.Request,
                    arg2: new() { StatusCode = LogoutResponseStatusCode.INPUT_VALIDATION_FAIL }
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
