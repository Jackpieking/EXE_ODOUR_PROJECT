using System;

namespace ODour.FastEndpointApi.Feature.Auth.ResendUserConfirmationEmail.HttpResponse;

internal sealed class LazyResendUserConfirmationEmailHttpResponseManager
{
    private static readonly Lazy<ResendUserConfirmationEmailHttpResponseManager> _resendUserConfirmationEmailHttpResponseManager =
        new();

    internal static ResendUserConfirmationEmailHttpResponseManager Get()
    {
        return _resendUserConfirmationEmailHttpResponseManager.Value;
    }
}
