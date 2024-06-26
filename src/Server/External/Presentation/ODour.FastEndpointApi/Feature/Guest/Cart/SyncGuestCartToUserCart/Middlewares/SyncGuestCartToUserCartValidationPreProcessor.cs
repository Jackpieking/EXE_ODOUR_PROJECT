using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Feature.Guest.Cart.SyncGuestCartToUserCart;
using ODour.FastEndpointApi.Feature.Guest.Cart.SyncGuestCartToUserCart.Common;
using ODour.FastEndpointApi.Feature.Guest.Cart.SyncGuestCartToUserCart.HttpResponse;

namespace ODour.FastEndpointApi.Feature.Guest.Cart.SyncGuestCartToUserCart.Middlewares;

internal sealed class SyncGuestCartToUserCartValidationPreProcessor
    : PreProcessor<EmptyRequest, SyncGuestCartToUserCartStateBag>
{
    public override async Task PreProcessAsync(
        IPreProcessorContext<EmptyRequest> context,
        SyncGuestCartToUserCartStateBag state,
        CancellationToken ct
    )
    {
        if (context.HasValidationFailures)
        {
            var httpResponse = SyncGuestCartToUserCartHttpResponseManager
                .Resolve(
                    statusCode: SyncGuestCartToUserCartResponseStatusCode.INPUT_VALIDATION_FAIL
                )
                .Invoke(
                    arg1: state.AppRequest,
                    arg2: new()
                    {
                        StatusCode = SyncGuestCartToUserCartResponseStatusCode.INPUT_VALIDATION_FAIL
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
