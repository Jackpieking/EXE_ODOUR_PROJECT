using System;
using System.Linq;
using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using ODour.Application.Share.Common;
using ODour.Application.Share.DataProtection;
using ODour.Application.Share.Tokens;

namespace ODour.FastEndpointApi.Shared.Authorization;

internal sealed class HangfireDashboardAdminKeyAuthorizationFilter : IDashboardAuthorizationFilter
{
    private const string CookieName = "_hgfAuth";
    private readonly Lazy<IAdminAccessKeyHandler> _adminAccessKeyHandler;
    private readonly Lazy<IDataProtectionHandler> _dataProtection;

    public HangfireDashboardAdminKeyAuthorizationFilter(
        Lazy<IAdminAccessKeyHandler> adminAccessKeyHandler,
        Lazy<IDataProtectionHandler> dataProtection
    )
    {
        _adminAccessKeyHandler = adminAccessKeyHandler;
        _dataProtection = dataProtection;
    }

    public bool Authorize([NotNull] DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        var adminKey = GetAdminKey(httpContext: httpContext);

        // Validate admin key.
        if (
            string.IsNullOrWhiteSpace(value: adminKey)
            || !adminKey.Equals(value: _adminAccessKeyHandler.Value.Get())
        )
        {
            return false;
        }

        SetNewCookieIfNotExist(httpContext: httpContext);

        return true;
    }

    private string GetAdminKey(HttpContext httpContext)
    {
        const string KeyQuery = "key";

        // Get admin key from key query or cookie.
        if (httpContext.Request.Query.ContainsKey(key: KeyQuery))
        {
            return httpContext.Request.Query[KeyQuery].FirstOrDefault();
        }
        else
        {
            var encyptedAdminKey = httpContext.Request.Cookies[CookieName];

            // cookie is empty.
            if (string.IsNullOrWhiteSpace(value: encyptedAdminKey))
            {
                return default;
            }

            // Decrypt admin key.
            var decryptedAdminKey = _dataProtection.Value.Unprotect(ciphertext: encyptedAdminKey);

            // invalid admin key.
            if (string.IsNullOrWhiteSpace(value: decryptedAdminKey))
            {
                return default;
            }

            return decryptedAdminKey.Split(separator: CommonConstant.App.DefaultStringSeparator)[1];
        }
    }

    private void SetNewCookieIfNotExist(HttpContext httpContext)
    {
        // Set cookie for admin session if does not exist.
        if (string.IsNullOrWhiteSpace(value: httpContext.Request.Cookies[CookieName]))
        {
            httpContext.Response.Cookies.Append(
                key: CookieName,
                value: _dataProtection.Value.Protect(
                    plaintext: $"{Guid.NewGuid()}{CommonConstant.App.DefaultStringSeparator}{_adminAccessKeyHandler.Value.Get()}"
                ),
                options: new() { Expires = DateTime.UtcNow.AddMinutes(value: 30) }
            );
        }
    }
}
