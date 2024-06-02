using System;

namespace ODour.FastEndpointApi.Feature.Auth.RegisterAsUser.HttpResponse;

/// <summary>
///     register as user extension methods.
/// </summary>
internal static class LazyRegisterAsUserHttpResponseMapper
{
    private static readonly Lazy<RegisterAsUserHttpResponseManager> _registerAsUserHttpResponseManager =
        new();

    internal static RegisterAsUserHttpResponseManager Get()
    {
        return _registerAsUserHttpResponseManager.Value;
    }
}
