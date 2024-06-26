using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Admin.Order.SwitchOrderStatusToDeliveringSuccessfully;
using ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToDeliveringSuccessfully.HttpResponse;
using ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToDeliveringSuccessfully.Middlewares;

namespace ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToDeliveringSuccessfully;

internal sealed class SwitchOrderStatusToDeliveringSuccessfullyEndpoint
    : Endpoint<
        SwitchOrderStatusToDeliveringSuccessfullyRequest,
        SwitchOrderStatusToDeliveringSuccessfullyHttpResponse
    >
{
    public override void Configure()
    {
        Post(routePatterns: "admin/orders/{orderId}/status-changing/delivering-successfully");
        AuthSchemes(authSchemeNames: JwtBearerDefaults.AuthenticationScheme);
        DontThrowIfValidationFails();
        PreProcessor<SwitchOrderStatusToDeliveringSuccessfullyValidationPreProcessor>();
        PreProcessor<SwitchOrderStatusToDeliveringSuccessfullyAuthorizationPreProcessor>();
        PreProcessor<SwitchOrderStatusToDeliveringSuccessfullyCachingPreProcessor>();
        PostProcessor<SwitchOrderStatusToDeliveringSuccessfullyCachingPostProcessor>();
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
            summary.Summary = "Endpoint for switch order status to delivering successfully feature";
            summary.Description =
                "This endpoint is used for switch order status to delivering successfully purpose.";
            summary.ExampleRequest = new SwitchOrderStatusToDeliveringSuccessfullyRequest
            {
                OrderId = Guid.Empty,
            };
            summary.Response<SwitchOrderStatusToDeliveringSuccessfullyHttpResponse>(
                description: "Represent successful operation response.",
                example: new()
                {
                    AppCode =
                        SwitchOrderStatusToDeliveringSuccessfullyResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
                }
            );
        });
    }

    public override async Task<SwitchOrderStatusToDeliveringSuccessfullyHttpResponse> ExecuteAsync(
        SwitchOrderStatusToDeliveringSuccessfullyRequest req,
        CancellationToken ct
    )
    {
        // Get app feature response.
        var appResponse = await req.ExecuteAsync(ct: ct);

        // Convert to http response.
        var httpResponse = SwitchOrderStatusToDeliveringSuccessfullyHttpResponseManager
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
