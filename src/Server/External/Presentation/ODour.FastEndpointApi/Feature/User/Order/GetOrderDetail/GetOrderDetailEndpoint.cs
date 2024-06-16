using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.User.Order.GetOrderDetail;
using ODour.FastEndpointApi.Feature.User.Order.GetOrderDetail.HttpResponse;
using ODour.FastEndpointApi.Feature.User.Order.GetOrderDetail.Middlewares;

namespace ODour.FastEndpointApi.Feature.User.Order.GetOrderDetail;

internal sealed class GetOrderDetailEndpoint
    : Endpoint<GetOrderDetailRequest, GetOrderDetailHttpResponse>
{
    public override void Configure()
    {
        Get(routePatterns: "user/order/{orderId}");
        AuthSchemes(authSchemeNames: JwtBearerDefaults.AuthenticationScheme);
        DontThrowIfValidationFails();
        PreProcessor<GetOrderDetailValidationPreProcessor>();
        PreProcessor<GetOrderDetailAuthorizationPreProcessor>();
        PreProcessor<GetOrderDetailCachingPreProcessor>();
        PostProcessor<GetOrderDetailCachingPostProcessor>();
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
            summary.Summary = "Endpoint for get order detail feature";
            summary.Description = "This endpoint is used for get order detail purpose.";
            summary.ExampleRequest = new();
            summary.Response<GetOrderDetailHttpResponse>(
                description: "Represent successful operation response.",
                example: new()
                {
                    AppCode = GetOrderDetailResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
                }
            );
        });
    }

    public override async Task<GetOrderDetailHttpResponse> ExecuteAsync(
        GetOrderDetailRequest req,
        CancellationToken ct
    )
    {
        // Get app feature response.
        var appResponse = await req.ExecuteAsync(ct: ct);

        // Convert to http response.
        var httpResponse = GetOrderDetailHttpResponseManager
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
