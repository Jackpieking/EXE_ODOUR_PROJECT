using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Feature.User.Cart.AddToCart;
using ODour.FastEndpointApi.Feature.User.Cart.AddToCart.Common;
using ODour.FastEndpointApi.Feature.User.Cart.AddToCart.HttpResponse;

namespace ODour.FastEndpointApi.Feature.User.Cart.AddToCart.Middlewares;

internal sealed class AddToCartValidationPreProcessor
    : PreProcessor<AddToCartRequest, AddToCartStateBag>
{
    public override async Task PreProcessAsync(
        IPreProcessorContext<AddToCartRequest> context,
        AddToCartStateBag state,
        CancellationToken ct
    )
    {
        if (context.HasValidationFailures)
        {
            var httpResponse = AddToCartHttpResponseManager
                .Resolve(statusCode: AddToCartResponseStatusCode.INPUT_VALIDATION_FAIL)
                .Invoke(
                    arg1: context.Request,
                    arg2: new() { StatusCode = AddToCartResponseStatusCode.INPUT_VALIDATION_FAIL }
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
