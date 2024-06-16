using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Feature.User.Order.GetOrderDetail;
using ODour.FastEndpointApi.Feature.User.Order.GetOrderDetail.Common;
using ODour.FastEndpointApi.Feature.User.Order.GetOrderDetail.HttpResponse;

namespace ODour.FastEndpointApi.Feature.User.Order.GetOrderDetail.Middlewares;

internal sealed class GetOrderDetailValidationPreProcessor
    : PreProcessor<GetOrderDetailRequest, GetOrderDetailStateBag>
{
    public override async Task PreProcessAsync(
        IPreProcessorContext<GetOrderDetailRequest> context,
        GetOrderDetailStateBag state,
        CancellationToken ct
    )
    {
        if (context.HasValidationFailures)
        {
            var httpResponse = GetOrderDetailHttpResponseManager
                .Resolve(statusCode: GetOrderDetailResponseStatusCode.INPUT_VALIDATION_FAIL)
                .Invoke(
                    arg1: context.Request,
                    arg2: new()
                    {
                        StatusCode = GetOrderDetailResponseStatusCode.INPUT_VALIDATION_FAIL
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
