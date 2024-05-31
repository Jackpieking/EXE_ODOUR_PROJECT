using System;

namespace ODour.FastEndpointApi.Feature.Auth.RegisterAsAdmin.HttpResponse;

internal static class LazyRegisterAsAdminHttpResponseManager
{
    private static readonly Lazy<RegisterAsAdminHttpResponseManager> _manager = new();

    internal static RegisterAsAdminHttpResponseManager Get()
    {
        return _manager.Value;
    }
}
