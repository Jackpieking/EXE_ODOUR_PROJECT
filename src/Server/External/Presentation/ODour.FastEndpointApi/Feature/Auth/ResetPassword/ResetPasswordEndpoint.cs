using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Auth.ResetPassword;
using ODour.FastEndpointApi.Feature.Auth.ResetPassword.HttpResponse;
using ODour.FastEndpointApi.Feature.Auth.ResetPassword.Middlewares;

namespace ODour.FastEndpointApi.Feature.Auth.ResetPassword;

internal sealed class ResetPasswordEndpoint
    : Endpoint<ResetPasswordRequest, ResetPasswordHttpResponse>
{
    public override void Configure()
    {
        Post(routePatterns: "auth/resetPassword");
        AllowAnonymous();
        DontThrowIfValidationFails();
        PreProcessor<ResetPasswordValidationPreProcessor>();
        Description(builder: builder =>
        {
            builder.ClearDefaultProduces(statusCodes: StatusCodes.Status400BadRequest);
        });
        Summary(endpointSummary: summary =>
        {
            summary.Summary = "Endpoint for reset password feature";
            summary.Description = "This endpoint is used for reset password purpose.";
            summary.ExampleRequest = new()
            {
                NewPassword = "string",
                ResetPasswordToken = "string"
            };
            summary.Response<ResetPasswordHttpResponse>(
                description: "Represent successful operation response.",
                example: new()
                {
                    AppCode = ResetPasswordResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
                }
            );
        });
    }

    public override async Task<ResetPasswordHttpResponse> ExecuteAsync(
        ResetPasswordRequest req,
        CancellationToken ct
    )
    {
        // Get app feature response.
        var appResponse = await req.ExecuteAsync(ct: ct);

        // Convert to http response.
        var httpResponse = LazyResetPasswordHttpResponseManager
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
