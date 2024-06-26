using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Admin.Order.SwitchOrderStatusToProcessing;
using ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToProcessing.HttpResponse;
using ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToProcessing.Middlewares;

namespace ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToProcessing;

internal sealed class SwitchOrderStatusToProcessingEndpoint
    : Endpoint<SwitchOrderStatusToProcessingRequest, SwitchOrderStatusToProcessingHttpResponse>
{
    public override void Configure()
    {
        Post(routePatterns: "admin/orders/{orderId}/status-changing/processing");
        AuthSchemes(authSchemeNames: JwtBearerDefaults.AuthenticationScheme);
        DontThrowIfValidationFails();
        PreProcessor<SwitchOrderStatusToProcessingValidationPreProcessor>();
        PreProcessor<SwitchOrderStatusToProcessingAuthorizationPreProcessor>();
        PreProcessor<SwitchOrderStatusToProcessingCachingPreProcessor>();
        PostProcessor<SwitchOrderStatusToProcessingCachingPostProcessor>();
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
            summary.Summary = "Endpoint for switch order status to processing feature";
            summary.Description =
                "This endpoint is used for switch order status to processing purpose.";
            summary.ExampleRequest = new SwitchOrderStatusToProcessingRequest
            {
                OrderId = Guid.Empty
            };
            summary.Response<SwitchOrderStatusToProcessingHttpResponse>(
                description: "Represent successful operation response.",
                example: new()
                {
                    AppCode =
                        SwitchOrderStatusToProcessingResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
                }
            );
        });
    }

    public override async Task<SwitchOrderStatusToProcessingHttpResponse> ExecuteAsync(
        SwitchOrderStatusToProcessingRequest req,
        CancellationToken ct
    )
    {
        // Get app feature response.
        var appResponse = await req.ExecuteAsync(ct: ct);

        // Convert to http response.
        var httpResponse = SwitchOrderStatusToProcessingHttpResponseManager
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
