using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Admin.Order.SwitchOrderStatus;
using ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatus.HttpResponse;
using ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatus.Middlewares;

namespace ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatus;

internal sealed class SwitchOrderStatusEndpoint
    : Endpoint<SwitchOrderStatusRequest, SwitchOrderStatusHttpResponse>
{
    public override void Configure()
    {
        Post(routePatterns: "admin/order/status/switch");
        AuthSchemes(authSchemeNames: JwtBearerDefaults.AuthenticationScheme);
        DontThrowIfValidationFails();
        PreProcessor<SwitchOrderStatusValidationPreProcessor>();
        PreProcessor<SwitchOrderStatusAuthorizationPreProcessor>();
        PreProcessor<SwitchOrderStatusCachingPreProcessor>();
        PostProcessor<SwitchOrderStatusCachingPostProcessor>();
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
            summary.Summary = "Endpoint for create new order feature";
            summary.Description = "This endpoint is used for create new order purpose.";
            summary.ExampleRequest = new SwitchOrderStatusRequest
            {
                OrderId = Guid.Empty,
                OrderStatusId = Guid.Empty
            };
            summary.Response<SwitchOrderStatusHttpResponse>(
                description: "Represent successful operation response.",
                example: new()
                {
                    AppCode = SwitchOrderStatusResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
                }
            );
        });
    }

    public override async Task<SwitchOrderStatusHttpResponse> ExecuteAsync(
        SwitchOrderStatusRequest req,
        CancellationToken ct
    )
    {
        // Get app feature response.
        var appResponse = await req.ExecuteAsync(ct: ct);

        // Convert to http response.
        var httpResponse = SwitchOrderStatusHttpResponseManager
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
