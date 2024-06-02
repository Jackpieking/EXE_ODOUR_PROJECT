using System;

namespace ODour.FastEndpointApi.Feature.Auth.Logout.HttpResponse;

internal static class LazyLogoutHttpResponseManager
{
    private static readonly Lazy<LogoutHttpResponseManager> _logoutHttpResponseManager = new();

    internal static LogoutHttpResponseManager Get()
    {
        return _logoutHttpResponseManager.Value;
    }
}
