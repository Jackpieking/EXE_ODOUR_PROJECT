using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.User.Cart.AddToCart;
using ODour.FastEndpointApi.Feature.User.Cart.AddToCart.HttpResponse;
using ODour.FastEndpointApi.Feature.User.Cart.AddToCart.Middlewares;

namespace ODour.FastEndpointApi.Feature.User.Cart.AddToCart;

internal sealed class AddToCartEndpoint : Endpoint<AddToCartRequest, AddToCartHttpResponse>
{
    public override void Configure()
    {
        Post(routePatterns: "user/cart/add");
        AuthSchemes(authSchemeNames: JwtBearerDefaults.AuthenticationScheme);
        DontThrowIfValidationFails();
        PreProcessor<AddToCartValidationPreProcessor>();
        PreProcessor<AddToCartAuthorizationPreProcessor>();
        PostProcessor<AddToCartCachingPostProcessor>();
        Description(builder: builder =>
        {
            builder.ClearDefaultProduces(
                StatusCodes.Status400BadRequest,
                StatusCodes.Status401Unauthorized,
                StatusCodes.Status403Forbidden
            );
        });
        Summary(endpointSummary: summary =>
        {
            summary.Summary = "Endpoint for add to cart feature";
            summary.Description = "This endpoint is used for add to cart purpose.";
            summary.ExampleRequest = new() { ProductId = "string", Quantity = 1 };
            summary.Response<AddToCartHttpResponse>(
                description: "Represent successful operation response.",
                example: new()
                {
                    AppCode = AddToCartResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
                }
            );
        });
    }

    public override async Task<AddToCartHttpResponse> ExecuteAsync(
        AddToCartRequest req,
        CancellationToken ct
    )
    {
        req.ProductId = req.ProductId.ToUpper();

        // Get app feature response.
        var appResponse = await req.ExecuteAsync(ct: ct);

        // Convert to http response.
        var httpResponse = AddToCartHttpResponseManager
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
