using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.User.Product.GetRelatedProductsByCategoryId;
using ODour.FastEndpointApi.Feature.User.Product.GetRelatedProductsByCategoryId.HttpResponse;
using ODour.FastEndpointApi.Feature.User.Product.GetRelatedProductsByCategoryId.Middlewares;

namespace ODour.FastEndpointApi.Feature.User.Product.GetRelatedProductsByCategoryId;

internal sealed class GetRelatedProductsByCategoryIdEndpoint
    : Endpoint<GetRelatedProductsByCategoryIdRequest, GetRelatedProductsByCategoryIdHttpResponse>
{
    public override void Configure()
    {
        Get(routePatterns: "product/related/{categoryId}");
        AllowAnonymous();
        DontThrowIfValidationFails();
        PreProcessor<GetRelatedProductsByCategoryIdValidationPreProcessor>();
        PreProcessor<GetRelatedProductsByCategoryIdCachingPreProcessor>();
        PostProcessor<GetRelatedProductsByCategoryIdCachingPostProcessor>();
        Description(builder: builder =>
        {
            builder.ClearDefaultProduces(statusCodes: StatusCodes.Status400BadRequest);
        });
        Summary(endpointSummary: summary =>
        {
            summary.Summary = "Endpoint for get related product by category id feature";
            summary.Description =
                "This endpoint is used for get related product by category id purpose.";
            summary.ExampleRequest = new() { CategoryId = Guid.Empty };
            summary.Response<GetRelatedProductsByCategoryIdHttpResponse>(
                description: "Represent successful operation response.",
                example: new()
                {
                    AppCode =
                        GetRelatedProductsByCategoryIdResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
                }
            );
        });
    }

    public override async Task<GetRelatedProductsByCategoryIdHttpResponse> ExecuteAsync(
        GetRelatedProductsByCategoryIdRequest req,
        CancellationToken ct
    )
    {
        // Get app feature response.
        var appResponse = await req.ExecuteAsync(ct: ct);

        // Convert to http response.
        var httpResponse = GetRelatedProductsByCategoryIdHttpResponseManager
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
