using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.User.Product.GetProductDetailByProductId;
using ODour.FastEndpointApi.Feature.User.Product.GetProductDetailByProductId.HttpResponse;
using ODour.FastEndpointApi.Feature.User.Product.GetProductDetailByProductId.Middlewares;

namespace ODour.FastEndpointApi.Feature.User.Product.GetProductDetailByProductId;

internal sealed class GetProductDetailByProductIdEndpoint
    : Endpoint<GetProductDetailByProductIdRequest, GetProductDetailByProductIdHttpResponse>
{
    public override void Configure()
    {
        Get(routePatterns: "product/{productId}");
        AllowAnonymous();
        DontThrowIfValidationFails();
        PreProcessor<GetProductDetailByProductIdValidationPreProcessor>();
        PreProcessor<GetProductDetailByProductIdCachingPreProcessor>();
        PostProcessor<GetProductDetailByProductIdCachingPostProcessor>();
        Description(builder: builder =>
        {
            builder.ClearDefaultProduces(statusCodes: StatusCodes.Status400BadRequest);
        });
        Summary(endpointSummary: summary =>
        {
            summary.Summary = "Endpoint for get product by id feature";
            summary.Description = "This endpoint is used for get product by id purpose.";
            summary.ExampleRequest = new() { ProductId = "string" };
            summary.Response<GetProductDetailByProductIdHttpResponse>(
                description: "Represent successful operation response.",
                example: new()
                {
                    AppCode =
                        GetProductDetailByProductIdResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
                }
            );
        });
    }

    public override async Task<GetProductDetailByProductIdHttpResponse> ExecuteAsync(
        GetProductDetailByProductIdRequest req,
        CancellationToken ct
    )
    {
        // Get app feature response.
        var appResponse = await req.ExecuteAsync(ct: ct);

        // Convert to http response.
        var httpResponse = GetProductDetailByProductIdHttpResponseManager
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

        // set http code of http response to default for not serializing.
        httpResponse.HttpCode = httpResponseStatusCode;

        return httpResponse;
    }
}
