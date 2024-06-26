using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Feature.User.Cart.GetCartDetail;
using ODour.Application.Feature.User.Order.CreateNewOrder;
using ODour.Application.Feature.User.Order.GetUserOrders;
using ODour.Application.Feature.User.Product.GetProductDetailByProductId;
using ODour.Application.Share.Caching;
using ODour.FastEndpointApi.Feature.User.Order.CreateNewOrder.Common;
using ODour.FastEndpointApi.Feature.User.Order.CreateNewOrder.HttpResponse;

namespace ODour.FastEndpointApi.Feature.User.Order.CreateNewOrder.Middlewares;

internal sealed class CreateNewOrderCachingPostProcessor
    : PostProcessor<CreateNewOrderRequest, CreateNewOrderStateBag, CreateNewOrderHttpResponse>
{
    private readonly Lazy<IServiceScopeFactory> _serviceScopeFactory;

    public CreateNewOrderCachingPostProcessor(Lazy<IServiceScopeFactory> serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override async Task PostProcessAsync(
        IPostProcessorContext<CreateNewOrderRequest, CreateNewOrderHttpResponse> context,
        CreateNewOrderStateBag state,
        CancellationToken ct
    )
    {
        if (Equals(objA: context.Response, objB: default))
        {
            return;
        }

        await using var scope = _serviceScopeFactory.Value.CreateAsyncScope();

        var cacheHandler = scope.Resolve<Lazy<ICacheHandler>>();

        // Clear cache if current app code is suitable.
        if (
            context.Response.AppCode.Equals(
                value: CreateNewOrderResponseStatusCode.OPERATION_SUCCESS.ToAppCode()
            )
        )
        {
            var removedCacheTasks = new List<Task>();

            foreach (var orderItem in context.Request.OrderItems)
            {
                removedCacheTasks.Add(
                    item: cacheHandler.Value.RemoveAsync(
                        key: $"{nameof(GetProductDetailByProductIdRequest)}__req__{orderItem.ProductId}",
                        cancellationToken: ct
                    )
                );
            }

            removedCacheTasks.AddRange(
                collection: new List<Task>(capacity: 2)
                {
                    cacheHandler.Value.RemoveAsync(
                        key: $"{nameof(GetUserOrdersRequest)}__req__{context.Request.GetUserId()}__{state.OrderStatusId}",
                        cancellationToken: ct
                    ),
                    cacheHandler.Value.RemoveAsync(
                        key: $"{nameof(GetCartDetailRequest)}__req__{context.Request.GetUserId()}",
                        cancellationToken: ct
                    )
                }
            );

            removedCacheTasks.TrimExcess();

            await Task.WhenAll(tasks: removedCacheTasks);
        }
    }
}
