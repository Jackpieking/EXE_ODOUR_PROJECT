using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.User.Product.GetAllProducts;
using ODour.FastEndpointApi.Feature.User.Product.GetAllProducts.HttpResponse;
using ODour.FastEndpointApi.Feature.User.Product.GetAllProducts.Middlewares;

namespace ODour.FastEndpointApi.Feature.User.Product.GetAllProducts;

internal sealed class GetAllProductsEndpoint
    : Endpoint<GetAllProductsRequest, GetAllProductsHttpResponse>
{
    public override void Configure()
    {
        Get(routePatterns: "product");
        AllowAnonymous();
        DontThrowIfValidationFails();
        PreProcessor<GetAllProductsValidationPreProcessor>();
        Description(builder: builder =>
        {
            builder.ClearDefaultProduces(statusCodes: StatusCodes.Status400BadRequest);
        });
        Summary(endpointSummary: summary =>
        {
            summary.Summary = "Endpoint for get all products feature";
            summary.Description = "This endpoint is used for get all products purpose.";
            summary.ExampleRequest = new() { CurrentPage = 0 };
            summary.Response<GetAllProductsHttpResponse>(
                description: "Represent successful operation response.",
                example: new()
                {
                    AppCode = GetAllProductsResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
                }
            );
        });
    }

    public override async Task<GetAllProductsHttpResponse> ExecuteAsync(
        GetAllProductsRequest req,
        CancellationToken ct
    )
    {
        // Get app feature response.
        var appResponse = await req.ExecuteAsync(ct: ct);

        // Convert to http response.
        var httpResponse = GetAllProductsHttpResponseManager
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
