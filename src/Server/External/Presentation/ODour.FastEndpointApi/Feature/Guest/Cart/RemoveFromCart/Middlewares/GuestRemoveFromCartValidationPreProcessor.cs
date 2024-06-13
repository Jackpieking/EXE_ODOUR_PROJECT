using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Feature.Guest.Cart.RemoveFromCart;
using ODour.FastEndpointApi.Feature.Guest.Cart.RemoveFromCart.Common;
using ODour.FastEndpointApi.Feature.Guest.Cart.RemoveFromCart.HttpResponse;

namespace ODour.FastEndpointApi.Feature.Guest.Cart.RemoveFromCart.Middlewares;

internal sealed class GuestRemoveFromCartValidationPreProcessor
    : PreProcessor<GuestRemoveFromCartRequest, GuestRemoveFromCartStateBag>
{
    public override async Task PreProcessAsync(
        IPreProcessorContext<GuestRemoveFromCartRequest> context,
        GuestRemoveFromCartStateBag state,
        CancellationToken ct
    )
    {
        if (context.HasValidationFailures)
        {
            var httpResponse = GuestRemoveFromCartHttpResponseManager
                .Resolve(statusCode: GuestRemoveFromCartResponseStatusCode.INPUT_VALIDATION_FAIL)
                .Invoke(
                    arg1: context.Request,
                    arg2: new()
                    {
                        StatusCode = GuestRemoveFromCartResponseStatusCode.INPUT_VALIDATION_FAIL
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
