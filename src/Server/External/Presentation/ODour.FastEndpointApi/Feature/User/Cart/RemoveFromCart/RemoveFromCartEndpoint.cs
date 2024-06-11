using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.User.Cart.RemoveFromCart;
using ODour.FastEndpointApi.Feature.User.Cart.RemoveFromCart.HttpResponse;
using ODour.FastEndpointApi.Feature.User.Cart.RemoveFromCart.Middlewares;

namespace ODour.FastEndpointApi.Feature.User.Cart.RemoveFromCart;

internal sealed class RemoveFromCartEndpoint
    : Endpoint<RemoveFromCartRequest, RemoveFromCartHttpResponse>
{
    public override void Configure()
    {
        Post(routePatterns: "user/cart/remove");
        AuthSchemes(authSchemeNames: JwtBearerDefaults.AuthenticationScheme);
        DontThrowIfValidationFails();
        PreProcessor<RemoveFromCartValidationPreProcessor>();
        PreProcessor<RemoveFromCartAuthorizationPreProcessor>();
        PostProcessor<RemoveFromCartCachingPostProcessor>();
        Description(builder: builder =>
        {
            builder.ClearDefaultProduces(statusCodes: StatusCodes.Status400BadRequest);
        });
        Summary(endpointSummary: summary =>
        {
            summary.Summary = "Endpoint for remove from cart feature";
            summary.Description = "This endpoint is used for remove from cart purpose.";
            summary.ExampleRequest = new() { ProductId = "string", Quantity = 1 };
            summary.Response<RemoveFromCartHttpResponse>(
                description: "Represent successful operation response.",
                example: new()
                {
                    AppCode = RemoveFromCartResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
                }
            );
        });
    }

    public override async Task<RemoveFromCartHttpResponse> ExecuteAsync(
        RemoveFromCartRequest req,
        CancellationToken ct
    )
    {
        req.ProductId = req.ProductId.ToUpper();

        // Get app feature response.
        var appResponse = await req.ExecuteAsync(ct: ct);

        // Convert to http response.
        var httpResponse = RemoveFromCartHttpResponseManager
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

        return httpResponse;
    }
}
