using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Auth.ConfirmUserEmail;
using ODour.FastEndpointApi.Feature.Auth.ConfirmUserEmail.HttpResponse;
using ODour.FastEndpointApi.Feature.Auth.ConfirmUserEmail.Middlewares;

namespace ODour.FastEndpointApi.Feature.Auth.ConfirmUserEmail;

internal sealed class ConfirmUserEmailEndpoint
    : Endpoint<ConfirmUserEmailRequest, ConfirmUserEmailHttpResponse>
{
    public override void Configure()
    {
        Post(routePatterns: "auth/confirmEmail");
        AllowAnonymous();
        DontThrowIfValidationFails();
        PreProcessor<ConfirmUserEmailValidationPreProcessor>();
        Description(builder: builder =>
        {
            builder.ClearDefaultProduces(statusCodes: StatusCodes.Status400BadRequest);
        });
        Summary(endpointSummary: summary =>
        {
            summary.Summary = "Endpoint for confirm user email feature";
            summary.Description = "This endpoint is used for confirm user email purpose.";
            summary.ExampleRequest = new() { Token = "string" };
            summary.Response<ConfirmUserEmailHttpResponse>(
                description: "Represent successful operation response.",
                example: new()
                {
                    AppCode = ConfirmUserEmailResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
                }
            );
        });
    }

    public override async Task<ConfirmUserEmailHttpResponse> ExecuteAsync(
        ConfirmUserEmailRequest req,
        CancellationToken ct
    )
    {
        // Get app feature response.
        var appResponse = await req.ExecuteAsync(ct: ct);

        // Convert to http response.
        var httpResponse = LazyConfirmUserEmailHttpResponseManager
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
