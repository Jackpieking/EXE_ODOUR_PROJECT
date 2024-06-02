using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Auth.RegisterAsAdmin;
using ODour.FastEndpointApi.Feature.Auth.RegisterAsAdmin.HttpResponse;
using ODour.FastEndpointApi.Feature.Auth.RegisterAsAdmin.Middleware;

namespace ODour.FastEndpointApi.Feature.Auth.RegisterAsAdmin;

internal sealed class RegisterAsAdminEndpoint
    : Endpoint<RegisterAsAdminRequest, RegisterAsAdminHttpResponse>
{
    public override void Configure()
    {
        Post(routePatterns: "admin/auth/register");
        AllowAnonymous();
        DontThrowIfValidationFails();
        PreProcessor<RegisterAsAdminValidationPreProcessor>();
        Description(builder: builder =>
        {
            builder.ClearDefaultProduces(statusCodes: StatusCodes.Status400BadRequest);
        });
        Summary(endpointSummary: summary =>
        {
            summary.Summary = "Endpoint for admin register/signup feature";
            summary.Description = "This endpoint is used for admin register/signup purpose.";
            summary.ExampleRequest = new()
            {
                Email = "string",
                Password = "string",
                AdminConfirmedKey = "string"
            };
            summary.Response<RegisterAsAdminHttpResponse>(
                description: "Represent successful operation response.",
                example: new()
                {
                    AppCode = RegisterAsAdminResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
                }
            );
        });
    }

    public override async Task<RegisterAsAdminHttpResponse> ExecuteAsync(
        RegisterAsAdminRequest req,
        CancellationToken ct
    )
    {
        // Get app feature response.
        var appResponse = await req.ExecuteAsync(ct: ct);

        // Convert to http response.
        var httpResponse = LazyRegisterAsAdminHttpResponseManager
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
