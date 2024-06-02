using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Auth.Login;
using ODour.FastEndpointApi.Feature.Auth.Login.HttpResponse;
using ODour.FastEndpointApi.Feature.Auth.Login.Middlewares;

namespace ODour.FastEndpointApi.Feature.Auth.Login;

internal sealed class LoginEndpoint : Endpoint<LoginRequest, LoginHttpResponse>
{
    public override void Configure()
    {
        Post(routePatterns: "auth/login");
        AllowAnonymous();
        DontThrowIfValidationFails();
        PreProcessor<LoginValidationPreProcessor>();
        Description(builder: builder =>
        {
            builder.ClearDefaultProduces(statusCodes: StatusCodes.Status400BadRequest);
        });
        Summary(endpointSummary: summary =>
        {
            summary.Summary = "Endpoint for user login feature";
            summary.Description = "This endpoint is used for user login purpose.";
            summary.ExampleRequest = new()
            {
                Email = "string",
                Password = "string",
                IsRememberMe = true
            };
            summary.Response<LoginHttpResponse>(
                description: "Represent successful operation response.",
                example: new() { AppCode = LoginResponseStatusCode.OPERATION_SUCCESS.ToAppCode() }
            );
        });
    }

    public override async Task<LoginHttpResponse> ExecuteAsync(
        LoginRequest req,
        CancellationToken ct
    )
    {
        // Get app feature response.
        var appResponse = await req.ExecuteAsync(ct: ct);

        // Convert to http response.
        var httpResponse = LazyLoginHttpResponseManager
            .Get()
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

        return httpResponse;
    }
}
