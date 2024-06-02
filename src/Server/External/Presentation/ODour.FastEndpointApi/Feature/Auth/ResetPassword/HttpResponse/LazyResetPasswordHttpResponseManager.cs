using System;

namespace ODour.FastEndpointApi.Feature.Auth.ResetPassword.HttpResponse;

internal static class LazyResetPasswordHttpResponseManager
{
    private static readonly Lazy<ResetPasswordHttpResponseManager> _resetPasswordHttpResponseManager =
        new();

    internal static ResetPasswordHttpResponseManager Get()
    {
        return _resetPasswordHttpResponseManager.Value;
    }
}
