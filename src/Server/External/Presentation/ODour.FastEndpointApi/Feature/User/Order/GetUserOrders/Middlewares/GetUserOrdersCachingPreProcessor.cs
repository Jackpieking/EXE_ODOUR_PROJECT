using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Feature.User.Order.GetUserOrders;
using ODour.Application.Share.Caching;
using ODour.FastEndpointApi.Feature.User.Order.GetUserOrders.Common;
using ODour.FastEndpointApi.Feature.User.Order.GetUserOrders.HttpResponse;

namespace ODour.FastEndpointApi.Feature.User.Order.GetUserOrders.Middlewares;

internal sealed class GetUserOrdersCachingPreProcessor
    : PreProcessor<GetUserOrdersRequest, GetUserOrdersStateBag>
{
    private readonly Lazy<IServiceScopeFactory> _serviceScopeFactory;

    public GetUserOrdersCachingPreProcessor(Lazy<IServiceScopeFactory> serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override async Task PreProcessAsync(
        IPreProcessorContext<GetUserOrdersRequest> context,
        GetUserOrdersStateBag state,
        CancellationToken ct
    )
    {
        if (context.HttpContext.ResponseStarted())
        {
            return;
        }

        state.CacheKey =
            $"{nameof(GetUserOrdersRequest)}__req__{context.Request.GetUserId()}__{context.Request.OrderStatusId}";

        await using var scope = _serviceScopeFactory.Value.CreateAsyncScope();

        var cacheHandler = scope.Resolve<Lazy<ICacheHandler>>();

        var cacheModel = await cacheHandler.Value.GetAsync<GetUserOrdersHttpResponse>(
            key: state.CacheKey,
            cancellationToken: ct
        );

        if (!Equals(objA: cacheModel, objB: AppCacheModel<GetUserOrdersHttpResponse>.NotFound))
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
