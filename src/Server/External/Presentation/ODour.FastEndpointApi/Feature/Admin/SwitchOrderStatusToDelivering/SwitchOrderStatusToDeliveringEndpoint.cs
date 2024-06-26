using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Admin.Order.SwitchOrderStatusToDelivering;
using ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToDelivering.HttpResponse;
using ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToDelivering.Middlewares;

namespace ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToDelivering;

internal sealed class SwitchOrderStatusToDeliveringEndpoint
    : Endpoint<SwitchOrderStatusToDeliveringRequest, SwitchOrderStatusToDeliveringHttpResponse>
{
    public override void Configure()
    {
        Post(routePatterns: "admin/orders/{orderId}/status-changing/delivering");
        AuthSchemes(authSchemeNames: JwtBearerDefaults.AuthenticationScheme);
        DontThrowIfValidationFails();
        PreProcessor<SwitchOrderStatusToDeliveringValidationPreProcessor>();
        PreProcessor<SwitchOrderStatusToDeliveringAuthorizationPreProcessor>();
        PreProcessor<SwitchOrderStatusToDeliveringCachingPreProcessor>();
        PostProcessor<SwitchOrderStatusToDeliveringCachingPostProcessor>();
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
            summary.Summary = "Endpoint for switch order status to delivering feature";
            summary.Description =
                "This endpoint is used for switch order status to delivering purpose.";
            summary.ExampleRequest = new SwitchOrderStatusToDeliveringRequest
            {
                OrderId = Guid.Empty,
            };
            summary.Response<SwitchOrderStatusToDeliveringHttpResponse>(
                description: "Represent successful operation response.",
                example: new()
                {
                    AppCode =
                        SwitchOrderStatusToDeliveringResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
                }
            );
        });
    }

    public override async Task<SwitchOrderStatusToDeliveringHttpResponse> ExecuteAsync(
        SwitchOrderStatusToDeliveringRequest req,
        CancellationToken ct
    )
    {
        // Get app feature response.
        var appResponse = await req.ExecuteAsync(ct: ct);

        // Convert to http response.
        var httpResponse = SwitchOrderStatusToDeliveringHttpResponseManager
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
