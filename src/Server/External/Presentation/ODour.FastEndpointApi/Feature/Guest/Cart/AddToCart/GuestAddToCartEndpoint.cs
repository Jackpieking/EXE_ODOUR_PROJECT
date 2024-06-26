using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Guest.Cart.AddToCart;
using ODour.FastEndpointApi.Feature.Guest.Cart.AddToCart.HttpResponse;
using ODour.FastEndpointApi.Feature.Guest.Cart.AddToCart.Middlewares;

namespace ODour.FastEndpointApi.Feature.Guest.Cart.AddToCart;

internal sealed class GuestAddToCartEndpoint
    : Endpoint<GuestAddToCartRequest, GuestAddToCartHttpResponse>
{
    public override void Configure()
    {
        Post(routePatterns: "guest/cart/add");
        AllowAnonymous();
        DontThrowIfValidationFails();
        PreProcessor<GuestAddToCartValidationPreProcessor>();
        PostProcessor<GuestAddToCartCachingPostProcessor>();
        Description(builder: builder =>
        {
            builder.ClearDefaultProduces(statusCodes: StatusCodes.Status400BadRequest);
        });
        Summary(endpointSummary: summary =>
        {
            summary.Summary = "Endpoint for guest add to cart feature";
            summary.Description = "This endpoint is used for guest add to cart purpose.";
            summary.ExampleRequest = new() { ProductId = "string", Quantity = 1 };
            summary.Response<GuestAddToCartHttpResponse>(
                description: "Represent successful operation response.",
                example: new()
                {
                    AppCode = GuestAddToCartResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
                }
            );
        });
    }

    public override async Task<GuestAddToCartHttpResponse> ExecuteAsync(
        GuestAddToCartRequest req,
        CancellationToken ct
    )
    {
        req.ProductId = req.ProductId.ToUpper();

        // Get app feature response.
        var appResponse = await req.ExecuteAsync(ct: ct);

        // Convert to http response.
        var httpResponse = GuestAddToCartHttpResponseManager
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
