using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Auth.RegisterAsUser;
using ODour.FastEndpointApi.Feature.Auth.RegisterAsUser.HttpResponse;
using ODour.FastEndpointApi.Feature.Auth.RegisterAsUser.Middlewares.Validation;

namespace ODour.FastEndpointApi.Feature.Auth.RegisterAsUser;

internal sealed class RegisterAsUserEndpoint
    : Endpoint<RegisterAsUserRequest, RegisterAsUserHttpResponse>
{
    public override void Configure()
    {
        Post(routePatterns: "auth/register");
        AllowAnonymous();
        DontThrowIfValidationFails();
        PreProcessor<RegisterAsUserValidationPreProcessor>();
        Description(builder: builder =>
        {
            builder.ClearDefaultProduces(statusCodes: StatusCodes.Status400BadRequest);
        });
        Summary(endpointSummary: summary =>
        {
            summary.Summary = "Endpoint for user register/signup feature";
            summary.Description = "This endpoint is used for user register/signup purpose.";
            summary.ExampleRequest = new() { Email = "string", Password = "string" };
            summary.Response<RegisterAsUserHttpResponse>(
                description: "Represent successful operation response.",
                example: new()
                {
                    AppCode = RegisterAsUserResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
                }
            );
        });
    }

    public override async Task<RegisterAsUserHttpResponse> ExecuteAsync(
        RegisterAsUserRequest req,
        CancellationToken ct
    )
    {
        // Get app feature response.
        var appResponse = await req.ExecuteAsync(ct: ct);

        // Convert to http response.
        var httpResponse = RegisterAsUserHttpResponseManager
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
