using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Feature.Guest.Cart.AddToCart;
using ODour.FastEndpointApi.Feature.Guest.Cart.AddToCart.Common;
using ODour.FastEndpointApi.Feature.Guest.Cart.AddToCart.HttpResponse;

namespace ODour.FastEndpointApi.Feature.Guest.Cart.AddToCart.Middlewares;

internal sealed class GuestAddToCartValidationPreProcessor
    : PreProcessor<GuestAddToCartRequest, GuestAddToCartStateBag>
{
    public override async Task PreProcessAsync(
        IPreProcessorContext<GuestAddToCartRequest> context,
        GuestAddToCartStateBag state,
        CancellationToken ct
    )
    {
        if (context.HasValidationFailures)
        {
            var httpResponse = GuestAddToCartHttpResponseManager
                .Resolve(statusCode: GuestAddToCartResponseStatusCode.INPUT_VALIDATION_FAIL)
                .Invoke(
                    arg1: context.Request,
                    arg2: new()
                    {
                        StatusCode = GuestAddToCartResponseStatusCode.INPUT_VALIDATION_FAIL
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
