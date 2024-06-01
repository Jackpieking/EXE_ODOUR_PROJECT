using System;
using Microsoft.AspNetCore.DataProtection;
using ODour.Application.Share.DataProtection;
using ODour.Configuration.Presentation.WebApi.SecurityKey;

namespace ODour.AppIdentityService.Handlers;

internal sealed class AppDataProtectionHandler : IDataProtectionHandler
{
    private readonly IDataProtector _protector;

    public AppDataProtectionHandler(
        IDataProtectionProvider dataProtectionProvider,
        Lazy<AppBaseProtectionSecurityKeyOption> options
    )
    {
        _protector = dataProtectionProvider.CreateProtector(purpose: options.Value.Value);
    }

    public string Protect(string plaintext)
    {
        if (string.IsNullOrWhiteSpace(value: plaintext))
        {
            return default;
        }

        return _protector.Protect(plaintext: plaintext);
    }

    public string Unprotect(string ciphertext)
    {
        if (string.IsNullOrWhiteSpace(value: ciphertext))
        {
            return default;
        }

        try
        {
            return _protector.Unprotect(protectedData: ciphertext);
        }
        catch
        {
            return default;
        }
    }
}
