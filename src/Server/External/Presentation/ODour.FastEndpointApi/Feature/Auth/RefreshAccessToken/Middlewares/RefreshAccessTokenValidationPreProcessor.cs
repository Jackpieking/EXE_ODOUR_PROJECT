using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Feature.Auth.RefreshAccessToken;
using ODour.FastEndpointApi.Feature.Auth.RefreshAccessToken.Common;
using ODour.FastEndpointApi.Feature.Auth.RefreshAccessToken.HttpResponse;

namespace ODour.FastEndpointApi.Feature.Auth.RefreshAccessToken.Middlewares;

internal sealed class RefreshAccessTokenValidationPreProcessor
    : PreProcessor<RefreshAccessTokenRequest, RefreshAccessTokenStateBag>
{
    public override async Task PreProcessAsync(
        IPreProcessorContext<RefreshAccessTokenRequest> context,
        RefreshAccessTokenStateBag state,
        CancellationToken ct
    )
    {
        if (context.HasValidationFailures)
        {
            var httpResponse = RefreshAccessTokenHttpResponseManager
                .Resolve(statusCode: RefreshAccessTokenResponseStatusCode.INPUT_VALIDATION_FAIL)
                .Invoke(
                    arg1: context.Request,
                    arg2: new()
                    {
                        StatusCode = RefreshAccessTokenResponseStatusCode.INPUT_VALIDATION_FAIL
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
