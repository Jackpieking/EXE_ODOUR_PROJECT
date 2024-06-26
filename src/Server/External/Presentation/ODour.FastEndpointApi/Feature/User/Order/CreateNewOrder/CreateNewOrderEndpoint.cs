using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using ODour.Application.Feature.User.Order.CreateNewOrder;
using ODour.FastEndpointApi.Feature.User.Order.CreateNewOrder.Common;
using ODour.FastEndpointApi.Feature.User.Order.CreateNewOrder.HttpResponse;
using ODour.FastEndpointApi.Feature.User.Order.CreateNewOrder.Middlewares;

namespace ODour.FastEndpointApi.Feature.User.Order.CreateNewOrder;

internal sealed class CreateNewOrderEndpoint
    : Endpoint<CreateNewOrderRequest, CreateNewOrderHttpResponse>
{
    public override void Configure()
    {
        Post(routePatterns: "user/order");
        AuthSchemes(authSchemeNames: JwtBearerDefaults.AuthenticationScheme);
        DontThrowIfValidationFails();
        PreProcessor<CreateNewOrderValidationPreProcessor>();
        PreProcessor<CreateNewOrderAuthorizationPreProcessor>();
        PostProcessor<CreateNewOrderCachingPostProcessor>();
        Description(builder: builder =>
        {
            builder.ClearDefaultProduces(
                StatusCodes.Status400BadRequest,
                StatusCodes.Status401Unauthorized,
                StatusCodes.Status403Forbidden
            );
        });
        Summary(endpointSummary: summary =>
        {
            summary.Summary = "Endpoint for create new order feature";
            summary.Description = "This endpoint is used for create new order purpose.";
            summary.ExampleRequest = new()
            {
                FullName = "string",
                PhoneNumber = "string",
                DeliveredAddress = "string",
                OrderNote = "string",
                PaymentMethodId = Guid.Empty,
                OrderItems = new List<CreateNewOrderRequest.OrderItem>(capacity: 1)
                {
                    new() { ProductId = "string", Quantity = default }
                }
            };
            summary.Response<CreateNewOrderHttpResponse>(
                description: "Represent successful operation response.",
                example: new()
                {
                    AppCode = CreateNewOrderResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
                }
            );
        });
    }

    public override async Task<CreateNewOrderHttpResponse> ExecuteAsync(
        CreateNewOrderRequest req,
        CancellationToken ct
    )
    {
        // Normalize product ids.
        foreach (var orderItem in req.OrderItems)
        {
            orderItem.ProductId = orderItem.ProductId.ToUpper();
        }

        // Get app feature response.
        var appResponse = await req.ExecuteAsync(ct: ct);

        // Convert to http response.
        var httpResponse = CreateNewOrderHttpResponseManager
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

        var stateBag = ProcessorState<CreateNewOrderStateBag>();

        stateBag.OrderStatusId = appResponse.OrderStatusId;

        return httpResponse;
    }
}
