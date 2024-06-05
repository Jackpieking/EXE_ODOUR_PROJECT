using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Auth.ResendUserConfirmationEmail;
using ODour.FastEndpointApi.Feature.Auth.ResendUserConfirmationEmail.HttpResponse;
using ODour.FastEndpointApi.Feature.Auth.ResendUserConfirmationEmail.Middlewares;

namespace ODour.FastEndpointApi.Feature.Auth.ResendUserConfirmationEmail;

internal sealed class ResendUserConfirmationEmailEndpoint
    : Endpoint<ResendUserConfirmationEmailRequest, ResendUserConfirmationEmailHttpResponse>
{
    public override void Configure()
    {
        Post(routePatterns: "auth/resendConfirmationEmail");
        AllowAnonymous();
        DontThrowIfValidationFails();
        PreProcessor<ResendUserConfirmationEmailValidationPreProcessor>();
        Description(builder: builder =>
        {
            builder.ClearDefaultProduces(statusCodes: StatusCodes.Status400BadRequest);
        });
        Summary(endpointSummary: summary =>
        {
            summary.Summary = "Endpoint for resend user confirmation email feature";
            summary.Description =
                "This endpoint is used for resend user confirmation email purpose.";
            summary.ExampleRequest = new() { Email = "string" };
            summary.Response<ResendUserConfirmationEmailHttpResponse>(
                description: "Represent successful operation response.",
                example: new()
                {
                    AppCode =
                        ResendUserConfirmationEmailResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
                }
            );
        });
    }

    public override async Task<ResendUserConfirmationEmailHttpResponse> ExecuteAsync(
        ResendUserConfirmationEmailRequest req,
        CancellationToken ct
    )
    {
        // Get app feature response.
        var appResponse = await req.ExecuteAsync(ct: ct);

        // Convert to http response.
        var httpResponse = ResendUserConfirmationEmailHttpResponseManager
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
