using System;

namespace ODour.FastEndpointApi.Feature.Auth.ForgotPassword.HttpResponse;

internal sealed class LazyForgotPasswordHttpResponseManager
{
    private static readonly Lazy<ForgotPasswordHttpResponseManager> _forgotPasswordHttpResponseManager =
        new();

    internal static ForgotPasswordHttpResponseManager Get()
    {
        return _forgotPasswordHttpResponseManager.Value;
    }
}
