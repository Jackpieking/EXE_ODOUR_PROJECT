using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.Auth.RefreshAccessToken;
using ODour.FastEndpointApi.Feature.Auth.RefreshAccessToken.Common;
using ODour.FastEndpointApi.Feature.Auth.RefreshAccessToken.HttpResponse;
using ODour.FastEndpointApi.Feature.Auth.RefreshAccessToken.Middlewares;

namespace ODour.FastEndpointApi.Feature.Auth.RefreshAccessToken;

internal sealed class RefreshAccessTokenEndpoint
    : Endpoint<RefreshAccessTokenRequest, RefreshAccessTokenHttpResponse>
{
    public override void Configure()
    {
        Post(routePatterns: "auth/refreshAccessToken");
        DontThrowIfValidationFails();
        AuthSchemes(authSchemeNames: JwtBearerDefaults.AuthenticationScheme);
        PreProcessor<RefreshAccessTokenValidationPreProcessor>();
        PreProcessor<RefreshAccessTokenAuthorizationPreProcessor>();
        Description(builder: builder =>
        {
            builder.ClearDefaultProduces(statusCodes: StatusCodes.Status400BadRequest);
        });
        Summary(endpointSummary: summary =>
        {
            summary.Summary = "Endpoint for user refresh new access token feature";
            summary.Description =
                "This endpoint is used for user refresh new access token purpose.";
            summary.ExampleRequest = new() { RefreshToken = "string" };
            summary.Response<RefreshAccessTokenHttpResponse>(
                description: "Represent successful operation response.",
                example: new()
                {
                    AppCode = RefreshAccessTokenResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
                }
            );
        });
    }

    public override async Task<RefreshAccessTokenHttpResponse> ExecuteAsync(
        RefreshAccessTokenRequest req,
        CancellationToken ct
    )
    {
        var stateBag = ProcessorState<RefreshAccessTokenStateBag>();

        req.SetUserId(userId: stateBag.FoundUserId);
        req.SetAccessTokenId(accessTokenId: stateBag.FoundAccessTokenId);

        // Get app feature response.
        var appResponse = await req.ExecuteAsync(ct: ct);

        // Convert to http response.
        var httpResponse = RefreshAccessTokenHttpResponseManager
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
