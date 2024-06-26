using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Feature.Guest.Cart.GetCartDetail;
using ODour.Application.Share.Caching;
using ODour.Application.Share.Common;
using ODour.FastEndpointApi.Feature.Guest.Cart.GetCartDetail.Common;
using ODour.FastEndpointApi.Feature.Guest.Cart.GetCartDetail.HttpResponse;

namespace ODour.FastEndpointApi.Feature.Guest.Cart.GetCartDetail.Middlewares;

internal sealed class GuestGetCartDetailCachingPreProcessor
    : PreProcessor<EmptyRequest, GuestGetCartDetailStateBag>
{
    private readonly Lazy<IServiceScopeFactory> _serviceScopeFactory;

    public GuestGetCartDetailCachingPreProcessor(Lazy<IServiceScopeFactory> serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override async Task PreProcessAsync(
        IPreProcessorContext<EmptyRequest> context,
        GuestGetCartDetailStateBag state,
        CancellationToken ct
    )
    {
        if (context.HttpContext.ResponseStarted())
        {
            return;
        }

        // Is any session key found?
        var isSessionKeyFound = context.HttpContext.Session.Keys.Any();

        if (!isSessionKeyFound)
        {
            state.CacheKey =
                $"{nameof(GuestGetCartDetailRequest)}__req__{CommonConstant.App.DefaultGuidValue}";
        }
        else
        {
            state.CacheKey =
                $"{nameof(GuestGetCartDetailRequest)}__req__{context.HttpContext.Session.Id}";
        }

        await using var scope = _serviceScopeFactory.Value.CreateAsyncScope();

        var cacheHandler = scope.Resolve<Lazy<ICacheHandler>>();

        var cacheModel = await cacheHandler.Value.GetAsync<GuestGetCartDetailHttpResponse>(
            key: state.CacheKey,
            cancellationToken: ct
        );

        if (!Equals(objA: cacheModel, objB: AppCacheModel<GuestGetCartDetailHttpResponse>.NotFound))
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
