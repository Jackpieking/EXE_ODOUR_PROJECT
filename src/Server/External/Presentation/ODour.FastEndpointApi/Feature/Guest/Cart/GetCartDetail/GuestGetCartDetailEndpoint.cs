using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Guest.Cart.GetCartDetail;
using ODour.FastEndpointApi.Feature.Guest.Cart.GetCartDetail.Common;
using ODour.FastEndpointApi.Feature.Guest.Cart.GetCartDetail.HttpResponse;
using ODour.FastEndpointApi.Feature.Guest.Cart.GetCartDetail.Middlewares;

namespace ODour.FastEndpointApi.Feature.Guest.Cart.GetCartDetail;

internal sealed class GuestGetCartDetailEndpoint
    : Endpoint<EmptyRequest, GuestGetCartDetailHttpResponse>
{
    public override void Configure()
    {
        Get(routePatterns: "guest/cart");
        AllowAnonymous();
        DontThrowIfValidationFails();
        PreProcessor<GuestGetCartDetailValidationPreProcessor>();
        PreProcessor<GuestGetCartDetailCachingPreProcessor>();
        PostProcessor<GuestGetCartDetailCachingPostProcessor>();
        Description(builder: builder =>
        {
            builder.ClearDefaultProduces(statusCodes: StatusCodes.Status400BadRequest);
        });
        Summary(endpointSummary: summary =>
        {
            summary.Summary = "Endpoint for get cart detail feature";
            summary.Description = "This endpoint is used for get cart detail purpose.";
            summary.ExampleRequest = new();
            summary.Response<GuestGetCartDetailHttpResponse>(
                description: "Represent successful operation response.",
                example: new()
                {
                    AppCode = GuestGetCartDetailResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
                }
            );
        });
    }

    public override async Task<GuestGetCartDetailHttpResponse> ExecuteAsync(
        EmptyRequest req,
        CancellationToken ct
    )
    {
        var stateBag = ProcessorState<GuestGetCartDetailStateBag>();

        // Get app feature response.
        var appResponse = await stateBag.AppRequest.ExecuteAsync(ct: ct);

        // Convert to http response.
        var httpResponse = GuestGetCartDetailHttpResponseManager
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
