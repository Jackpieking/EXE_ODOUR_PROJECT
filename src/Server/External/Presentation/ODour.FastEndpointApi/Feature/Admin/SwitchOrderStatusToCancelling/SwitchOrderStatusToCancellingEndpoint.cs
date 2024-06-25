using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Admin.Order.SwitchOrderStatusToCancelling;
using ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToCancelling.HttpResponse;
using ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToCancelling.Middlewares;

namespace ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToCancelling;

internal sealed class SwitchOrderStatusToCancellingEndpoint
    : Endpoint<SwitchOrderStatusToCancellingRequest, SwitchOrderStatusToCancellingHttpResponse>
{
    public override void Configure()
    {
        Post(routePatterns: "admin/orders/{orderId}/status-changing/cancelling");
        AuthSchemes(authSchemeNames: JwtBearerDefaults.AuthenticationScheme);
        DontThrowIfValidationFails();
        PreProcessor<SwitchOrderStatusToCancellingValidationPreProcessor>();
        PreProcessor<SwitchOrderStatusToCancellingAuthorizationPreProcessor>();
        PreProcessor<SwitchOrderStatusToCancellingCachingPreProcessor>();
        PostProcessor<SwitchOrderStatusToCancellingCachingPostProcessor>();
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
            summary.Summary = "Endpoint for switch order status to cancelling feature";
            summary.Description =
                "This endpoint is used for switch order status to cancelling purpose.";
            summary.ExampleRequest = new SwitchOrderStatusToCancellingRequest
            {
                OrderId = Guid.Empty,
            };
            summary.Response<SwitchOrderStatusToCancellingHttpResponse>(
                description: "Represent successful operation response.",
                example: new()
                {
                    AppCode =
                        SwitchOrderStatusToCancellingResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
                }
            );
        });
    }

    public override async Task<SwitchOrderStatusToCancellingHttpResponse> ExecuteAsync(
        SwitchOrderStatusToCancellingRequest req,
        CancellationToken ct
    )
    {
        // Get app feature response.
        var appResponse = await req.ExecuteAsync(ct: ct);

        // Convert to http response.
        var httpResponse = SwitchOrderStatusToCancellingHttpResponseManager
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
