using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.User.Product.GetProductsForHomePage;
using ODour.FastEndpointApi.Feature.User.Product.GetProductsForHomePage.Common;
using ODour.FastEndpointApi.Feature.User.Product.GetProductsForHomePage.HttpResponse;
using ODour.FastEndpointApi.Feature.User.Product.GetProductsForHomePage.Middlewares;

namespace ODour.FastEndpointApi.Feature.User.Product.GetProductsForHomePage;

internal sealed class GetProductsForHomePageEndpoint
    : Endpoint<EmptyRequest, GetProductsForHomePageHttpResponse>
{
    public override void Configure()
    {
        Get(routePatterns: "product/home");
        AllowAnonymous();
        DontThrowIfValidationFails();
        PreProcessor<GetProductsForHomePageCachingPreProcessor>();
        PostProcessor<GetProductsForHomePageCachingPostProcessor>();
        Description(builder: builder =>
        {
            builder.ClearDefaultProduces(statusCodes: StatusCodes.Status400BadRequest);
        });
        Summary(endpointSummary: summary =>
        {
            summary.Summary = "Endpoint for get all products feature";
            summary.Description = "This endpoint is used for get all products purpose.";
            summary.ExampleRequest = new();
            summary.Response<GetProductsForHomePageHttpResponse>(
                description: "Represent successful operation response.",
                example: new()
                {
                    AppCode = GetProductsForHomePageResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
                }
            );
        });
    }

    public override async Task<GetProductsForHomePageHttpResponse> ExecuteAsync(
        EmptyRequest req,
        CancellationToken ct
    )
    {
        var stateBag = ProcessorState<GetProductsForHomePageStateBag>();

        // Get app feature response.
        var appResponse = await stateBag.AppRequest.ExecuteAsync(ct: ct);

        // Convert to http response.
        var httpResponse = GetProductsForHomePageHttpResponseManager
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
