using System;

namespace ODour.FastEndpointApi.Feature.Auth.RefreshAccessToken.HttpResponse;

internal static class LazyRefreshAccessTokenHttpResponseManager
{
    private static readonly Lazy<RefreshAccessTokenHttpResponseManager> _refreshAccessTokenHttpResponseManager =
        new();

    internal static RefreshAccessTokenHttpResponseManager Get()
    {
        return _refreshAccessTokenHttpResponseManager.Value;
    }
}
