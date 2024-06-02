using System;

namespace ODour.FastEndpointApi.Feature.Auth.Login.HttpResponse;

internal static class LazyLoginHttpResponseManager
{
    private static readonly Lazy<LoginHttpResponseManager> _loginHttpResponseManager = new();

    internal static LoginHttpResponseManager Get()
    {
        return _loginHttpResponseManager.Value;
    }
}
