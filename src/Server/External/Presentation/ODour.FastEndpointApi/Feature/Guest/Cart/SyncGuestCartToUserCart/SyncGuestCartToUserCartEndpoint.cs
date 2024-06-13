using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Guest.Cart.SyncGuestCartToUserCart;
using ODour.FastEndpointApi.Feature.Guest.Cart.SyncGuestCartToUserCart.Common;
using ODour.FastEndpointApi.Feature.Guest.Cart.SyncGuestCartToUserCart.HttpResponse;
using ODour.FastEndpointApi.Feature.Guest.Cart.SyncGuestCartToUserCart.Middlewares;

namespace ODour.FastEndpointApi.Feature.Guest.Cart.SyncGuestCartToUserCart;

internal sealed class SyncGuestCartToUserCartEndpoint
    : Endpoint<EmptyRequest, SyncGuestCartToUserCartHttpResponse>
{
    public override void Configure()
    {
        Get(routePatterns: "guest/cart/sync");
        AuthSchemes(authSchemeNames: JwtBearerDefaults.AuthenticationScheme);
        DontThrowIfValidationFails();
        PreProcessor<SyncGuestCartToUserCartValidationPreProcessor>();
        PreProcessor<SyncGuestCartToUserCartAuthorizationPreProcessor>();
        PostProcessor<SyncGuestCartToUserCartCachingPostProcessor>();
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
            summary.Summary = "Endpoint for sync guest cart to user cart feature";
            summary.Description = "This endpoint is used for sync guest cart to user cart purpose.";
            summary.ExampleRequest = new();
            summary.Response<SyncGuestCartToUserCartHttpResponse>(
                description: "Represent successful operation response.",
                example: new()
                {
                    AppCode =
                        SyncGuestCartToUserCartResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
                }
            );
        });
    }

    public override async Task<SyncGuestCartToUserCartHttpResponse> ExecuteAsync(
        EmptyRequest req,
        CancellationToken ct
    )
    {
        var stateBag = ProcessorState<SyncGuestCartToUserCartStateBag>();

        // Get app feature response.
        var appResponse = await stateBag.AppRequest.ExecuteAsync(ct: ct);

        // Convert to http response.
        var httpResponse = SyncGuestCartToUserCartHttpResponseManager
            .Resolve(statusCode: appResponse.StatusCode)
            .Invoke(arg1: stateBag.AppRequest, arg2: appResponse);

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
