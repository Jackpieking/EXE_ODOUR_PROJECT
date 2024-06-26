using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Feature.User.Order.GetOrderDetail;
using ODour.Application.Share.Caching;
using ODour.FastEndpointApi.Feature.User.Order.GetOrderDetail.Common;
using ODour.FastEndpointApi.Feature.User.Order.GetOrderDetail.HttpResponse;

namespace ODour.FastEndpointApi.Feature.User.Order.GetOrderDetail.Middlewares;

internal sealed class GetOrderDetailCachingPreProcessor
    : PreProcessor<GetOrderDetailRequest, GetOrderDetailStateBag>
{
    private readonly Lazy<IServiceScopeFactory> _serviceScopeFactory;

    public GetOrderDetailCachingPreProcessor(Lazy<IServiceScopeFactory> serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override async Task PreProcessAsync(
        IPreProcessorContext<GetOrderDetailRequest> context,
        GetOrderDetailStateBag state,
        CancellationToken ct
    )
    {
        if (context.HttpContext.ResponseStarted())
        {
            return;
        }

        state.CacheKey = $"{nameof(GetOrderDetailRequest)}__req__{context.Request.OrderId}";

        await using var scope = _serviceScopeFactory.Value.CreateAsyncScope();

        var cacheHandler = scope.Resolve<Lazy<ICacheHandler>>();

        var cacheModel = await cacheHandler.Value.GetAsync<GetOrderDetailHttpResponse>(
            key: state.CacheKey,
            cancellationToken: ct
        );

        if (!Equals(objA: cacheModel, objB: AppCacheModel<GetOrderDetailHttpResponse>.NotFound))
        {
            var httpCode = cacheModel.Value.HttpCode;
            cacheModel.Value.HttpCode = default;

            await context.HttpContext.Response.SendAsync(
                response: cacheModel.Value,
                statusCode: httpCode,
                cancellation: ct
            );

            return;
        }
    }
}
