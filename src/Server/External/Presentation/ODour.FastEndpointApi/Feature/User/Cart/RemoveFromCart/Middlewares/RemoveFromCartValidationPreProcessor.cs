using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Feature.User.Cart.RemoveFromCart;
using ODour.FastEndpointApi.Feature.User.Cart.RemoveFromCart.Common;
using ODour.FastEndpointApi.Feature.User.Cart.RemoveFromCart.HttpResponse;

namespace ODour.FastEndpointApi.Feature.User.Cart.RemoveFromCart.Middlewares;

internal sealed class RemoveFromCartValidationPreProcessor
    : PreProcessor<RemoveFromCartRequest, RemoveFromCartStateBag>
{
    public override async Task PreProcessAsync(
        IPreProcessorContext<RemoveFromCartRequest> context,
        RemoveFromCartStateBag state,
        CancellationToken ct
    )
    {
        if (context.HasValidationFailures)
        {
            var httpResponse = RemoveFromCartHttpResponseManager
                .Resolve(statusCode: RemoveFromCartResponseStatusCode.INPUT_VALIDATION_FAIL)
                .Invoke(
                    arg1: context.Request,
                    arg2: new()
                    {
                        StatusCode = RemoveFromCartResponseStatusCode.INPUT_VALIDATION_FAIL
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
