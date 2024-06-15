using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Feature.User.Order.GetUserOrders;
using ODour.FastEndpointApi.Feature.User.Order.GetUserOrders.Common;
using ODour.FastEndpointApi.Feature.User.Order.GetUserOrders.HttpResponse;

namespace ODour.FastEndpointApi.Feature.User.Order.GetUserOrders.Middlewares;

internal sealed class GetUserOrdersValidationPreProcessor
    : PreProcessor<GetUserOrdersRequest, GetUserOrdersStateBag>
{
    public override async Task PreProcessAsync(
        IPreProcessorContext<GetUserOrdersRequest> context,
        GetUserOrdersStateBag state,
        CancellationToken ct
    )
    {
        if (context.HasValidationFailures)
        {
            var httpResponse = GetUserOrdersHttpResponseManager
                .Resolve(statusCode: GetUserOrdersResponseStatusCode.INPUT_VALIDATION_FAIL)
                .Invoke(
                    arg1: context.Request,
                    arg2: new()
                    {
                        StatusCode = GetUserOrdersResponseStatusCode.INPUT_VALIDATION_FAIL
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
