using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Auth.Logout;
using ODour.FastEndpointApi.Feature.Auth.Logout.Common;
using ODour.FastEndpointApi.Feature.Auth.Logout.HttpResponse;
using ODour.FastEndpointApi.Feature.Auth.Logout.Middlewares;

namespace ODour.FastEndpointApi.Feature.Auth.Logout;

internal sealed class LogoutEndpoint : Endpoint<EmptyRequest, LogoutHttpResponse>
{
    public override void Configure()
    {
        Post(routePatterns: "auth/logout");
        DontThrowIfValidationFails();
        AuthSchemes(authSchemeNames: JwtBearerDefaults.AuthenticationScheme);
        PreProcessor<LogoutValidationPreProcessor>();
        PreProcessor<LogoutAuthorizationPreProcessor>();
        PostProcessor<LogoutCachingPostProcessor>();
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
            summary.Summary = "Endpoint for user logout feature";
            summary.Description = "This endpoint is used for user logout purpose.";
            summary.ExampleRequest = new();
            summary.Response<LogoutHttpResponse>(
                description: "Represent successful operation response.",
                example: new() { AppCode = LogoutResponseStatusCode.OPERATION_SUCCESS.ToAppCode() }
            );
        });
    }

    public override async Task<LogoutHttpResponse> ExecuteAsync(
        EmptyRequest req,
        CancellationToken ct
    )
    {
        var stateBag = ProcessorState<LogoutStateBag>();

        // Get app feature response.
        var appResponse = await stateBag.AppRequest.ExecuteAsync(ct: ct);

        // Convert to http response.
        var httpResponse = LogoutHttpResponseManager
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
