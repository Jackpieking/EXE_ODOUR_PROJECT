using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Feature.User.Cart.GetCartDetail;
using ODour.Application.Share.Caching;
using ODour.FastEndpointApi.Feature.User.Cart.GetCartDetail.Common;
using ODour.FastEndpointApi.Feature.User.Cart.GetCartDetail.HttpResponse;

namespace ODour.FastEndpointApi.Feature.User.Cart.GetCartDetail.Middlewares;

internal sealed class GetCartDetailCachingPreProcessor
    : PreProcessor<EmptyRequest, GetCartDetailStateBag>
{
    private readonly Lazy<IServiceScopeFactory> _serviceScopeFactory;

    public GetCartDetailCachingPreProcessor(Lazy<IServiceScopeFactory> serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override async Task PreProcessAsync(
        IPreProcessorContext<EmptyRequest> context,
        GetCartDetailStateBag state,
        CancellationToken ct
    )
    {
        if (context.HttpContext.ResponseStarted())
        {
            return;
        }

        state.CacheKey = $"{nameof(GetCartDetailRequest)}__req__{state.AppRequest.GetUserId()}";

        await using var scope = _serviceScopeFactory.Value.CreateAsyncScope();

        var cacheHandler = scope.Resolve<Lazy<ICacheHandler>>();

        var cacheModel = await cacheHandler.Value.GetAsync<GetCartDetailHttpResponse>(
            key: state.CacheKey,
            cancellationToken: ct
        );

        if (!Equals(objA: cacheModel, objB: AppCacheModel<GetCartDetailHttpResponse>.NotFound))
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
