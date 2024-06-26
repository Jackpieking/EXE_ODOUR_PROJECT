using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Feature.User.Order.CreateNewOrder;
using ODour.FastEndpointApi.Feature.User.Order.CreateNewOrder.Common;
using ODour.FastEndpointApi.Feature.User.Order.CreateNewOrder.HttpResponse;

namespace ODour.FastEndpointApi.Feature.User.Order.CreateNewOrder.Middlewares;

internal sealed class CreateNewOrderValidationPreProcessor
    : PreProcessor<CreateNewOrderRequest, CreateNewOrderStateBag>
{
    public override async Task PreProcessAsync(
        IPreProcessorContext<CreateNewOrderRequest> context,
        CreateNewOrderStateBag state,
        CancellationToken ct
    )
    {
        if (context.HasValidationFailures)
        {
            var httpResponse = CreateNewOrderHttpResponseManager
                .Resolve(statusCode: CreateNewOrderResponseStatusCode.INPUT_VALIDATION_FAIL)
                .Invoke(
                    arg1: context.Request,
                    arg2: new()
                    {
                        StatusCode = CreateNewOrderResponseStatusCode.INPUT_VALIDATION_FAIL
                    }
                );

            // Save http code temporarily and set http code of response to default for not serializing.
            var httpCode = httpResponse.HttpCode;
            httpResponse.HttpCode = default;

            await context.HttpContext.Response.SendAsync(
                response: httpResponse,
                statusCode: httpCode,
                cancellation: ct
            );

            return;
        }
    }
}
