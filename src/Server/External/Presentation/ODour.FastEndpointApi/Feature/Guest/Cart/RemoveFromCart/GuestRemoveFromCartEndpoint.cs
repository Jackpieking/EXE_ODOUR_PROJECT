using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Guest.Cart.RemoveFromCart;
using ODour.FastEndpointApi.Feature.Guest.Cart.RemoveFromCart.HttpResponse;
using ODour.FastEndpointApi.Feature.Guest.Cart.RemoveFromCart.Middlewares;

namespace ODour.FastEndpointApi.Feature.Guest.Cart.RemoveFromCart;

internal sealed class GuestRemoveFromCartEndpoint
    : Endpoint<GuestRemoveFromCartRequest, GuestRemoveFromCartHttpResponse>
{
    public override void Configure()
    {
        Post(routePatterns: "guest/cart/remove");
        AllowAnonymous();
        DontThrowIfValidationFails();
        PreProcessor<GuestRemoveFromCartValidationPreProcessor>();
        PostProcessor<GuestRemoveFromCartCachingPostProcessor>();
        Description(builder: builder =>
        {
            builder.ClearDefaultProduces(statusCodes: StatusCodes.Status400BadRequest);
        });
        Summary(endpointSummary: summary =>
        {
            summary.Summary = "Endpoint for guest remove from cart feature";
            summary.Description = "This endpoint is used for guest remove from cart purpose.";
            summary.ExampleRequest = new() { ProductId = "string", Quantity = 1 };
            summary.Response<GuestRemoveFromCartHttpResponse>(
                description: "Represent successful operation response.",
                example: new()
                {
                    AppCode = GuestRemoveFromCartResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
                }
            );
        });
    }

    public override async Task<GuestRemoveFromCartHttpResponse> ExecuteAsync(
        GuestRemoveFromCartRequest req,
        CancellationToken ct
    )
    {
        req.ProductId = req.ProductId.ToUpper();

        // Get app feature response.
        var appResponse = await req.ExecuteAsync(ct: ct);

        // Convert to http response.
        var httpResponse = GuestRemoveFromCartHttpResponseManager
            .Resolve(statusCode: appResponse.StatusCode)
            .Invoke(arg1: req, arg2: appResponse);

        /*
         * Store the real http code of http response into a temporary variable.
         * Set the http code of http response to default for not serializing.
         */
        var httpResponseStatusCode = httpResponse.HttpCode;
        httpResponse.HttpCode = default;

        // Send http response to client.
        await SendAsync(
            response: httpResponse,
            statusCode: httpResponseStatusCode,
            cancellation: ct
        );

        // Set the http code of http response back.
        httpResponse.HttpCode = httpResponseStatusCode;

        return httpResponse;
    }
}
