using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.DataProtection;
using ODour.Application.Share.DataProtection;
using ODour.Configuration.Presentation.WebApi.SecurityKey;

namespace ODour.AppIdentityService.Handlers;

internal sealed class AppDataProtectionHandler : IDataProtectionHandler
{
    private readonly Dictionary<string, IDataProtector> _pairs = new(capacity: default);
    private readonly Lazy<IDataProtectionProvider> _dataProtectionProvider;
    private readonly Lazy<AppBaseProtectionSecurityKeyOption> _options;

    public AppDataProtectionHandler(
        Lazy<IDataProtectionProvider> dataProtectionProvider,
        Lazy<AppBaseProtectionSecurityKeyOption> options
    )
    {
        _pairs.Add(
            _options.Value.Value,
            _dataProtectionProvider.Value.CreateProtector(purpose: _options.Value.Value)
        );
        _dataProtectionProvider = dataProtectionProvider;
        _options = options;
    }

    public string Protect(string plaintext)
    {
        return _pairs[_options.Value.Value].Protect(plaintext: plaintext);
    }

    public string Protect(string key, string plaintext)
    {
        var encryptKey = $"{_options.Value.Value}<{key}>";

        if (_pairs.TryGetValue(key: encryptKey, value: out var dataProtector))
        {
            return dataProtector.Protect(plaintext: plaintext);
        }

        var newProtector = _dataProtectionProvider.Value.CreateProtector(purpose: encryptKey);

        _pairs.Add(key: encryptKey, value: newProtector);

        return newProtector.Protect(plaintext: plaintext);
    }

    public string Unprotect(string ciphertext)
    {
        return _pairs[_options.Value.Value].Unprotect(protectedData: ciphertext);
    }
}
