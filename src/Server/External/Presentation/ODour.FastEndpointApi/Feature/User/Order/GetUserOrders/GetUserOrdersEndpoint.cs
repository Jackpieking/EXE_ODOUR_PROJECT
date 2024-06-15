using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.User.Order.GetUserOrders;
using ODour.FastEndpointApi.Feature.User.Order.GetUserOrders.HttpResponse;
using ODour.FastEndpointApi.Feature.User.Order.GetUserOrders.Middlewares;

namespace ODour.FastEndpointApi.Feature.User.Order.GetUserOrders;

internal sealed class GetUserOrdersEndpoint
    : Endpoint<GetUserOrdersRequest, GetUserOrdersHttpResponse>
{
    public override void Configure()
    {
        Get(routePatterns: "user/order");
        AuthSchemes(authSchemeNames: JwtBearerDefaults.AuthenticationScheme);
        DontThrowIfValidationFails();
        PreProcessor<GetUserOrdersValidationPreProcessor>();
        PreProcessor<GetUserOrdersAuthorizationPreProcessor>();
        PreProcessor<GetUserOrdersCachingPreProcessor>();
        PostProcessor<GetUserOrdersCachingPostProcessor>();
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
            summary.Summary = "Endpoint for get user orders feature";
            summary.Description = "This endpoint is used for get user orders purpose.";
            summary.ExampleRequest = new();
            summary.Response<GetUserOrdersHttpResponse>(
                description: "Represent successful operation response.",
                example: new()
                {
                    AppCode = GetUserOrdersResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
                }
            );
        });
    }

    public override async Task<GetUserOrdersHttpResponse> ExecuteAsync(
        GetUserOrdersRequest req,
        CancellationToken ct
    )
    {
        // Get app feature response.
        var appResponse = await req.ExecuteAsync(ct: ct);

        // Convert to http response.
        var httpResponse = GetUserOrdersHttpResponseManager
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
