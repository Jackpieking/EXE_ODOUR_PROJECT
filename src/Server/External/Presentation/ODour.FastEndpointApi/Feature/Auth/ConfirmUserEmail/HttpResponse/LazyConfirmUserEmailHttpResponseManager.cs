using System;

namespace ODour.FastEndpointApi.Feature.Auth.ConfirmUserEmail.HttpResponse;

internal static class LazyConfirmUserEmailHttpResponseManager
{
    private static readonly Lazy<ConfirmUserEmailHttpResponseManager> _confirmUserEmailHttpResponseManager =
        new();

    internal static ConfirmUserEmailHttpResponseManager Get()
    {
        return _confirmUserEmailHttpResponseManager.Value;
    }
}