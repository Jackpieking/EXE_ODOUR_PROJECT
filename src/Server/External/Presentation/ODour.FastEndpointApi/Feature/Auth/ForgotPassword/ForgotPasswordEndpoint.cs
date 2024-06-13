using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Auth.ForgotPassword;
using ODour.FastEndpointApi.Feature.Auth.ForgotPassword.HttpResponse;
using ODour.FastEndpointApi.Feature.Auth.ForgotPassword.Middlewares;

namespace ODour.FastEndpointApi.Feature.Auth.ForgotPassword;

internal sealed class ForgotPasswordEndpoint
    : Endpoint<ForgotPasswordRequest, ForgotPasswordHttpResponse>
{
    public override void Configure()
    {
        Post(routePatterns: "auth/forgotPassword");
        AllowAnonymous();
        DontThrowIfValidationFails();
        PreProcessor<ForgotPasswordValidationPreProcessor>();
        Description(builder: builder =>
        {
            builder.ClearDefaultProduces(statusCodes: StatusCodes.Status400BadRequest);
        });
        Summary(endpointSummary: summary =>
        {
            summary.Summary = "Endpoint for user forgot password feature";
            summary.Description = "This endpoint is used for user forgot password purpose.";
            summary.ExampleRequest = new() { Email = "string" };
            summary.Response<ForgotPasswordHttpResponse>(
                description: "Represent successful operation response.",
                example: new()
                {
                    AppCode = ForgotPasswordResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
                }
            );
        });
    }

    public override async Task<ForgotPasswordHttpResponse> ExecuteAsync(
        ForgotPasswordRequest req,
        CancellationToken ct
    )
    {
        // Get app feature response.
        var appResponse = await req.ExecuteAsync(ct: ct);

        // Convert to http response.
        var httpResponse = ForgotPasswordHttpResponseManager
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
