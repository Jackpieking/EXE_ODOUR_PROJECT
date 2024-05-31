using System;
using ODour.Application.Share.Tokens;
using ODour.Configuration.Presentation.WebApi.SecurityKey;

namespace ODour.AppIdentityService.Handlers;

internal sealed class AppAdminAccessKeyHandler : IAdminAccessKeyHandler
{
    private readonly Lazy<AdminAccessSecurityKeyOption> _option;

    public AppAdminAccessKeyHandler(Lazy<AdminAccessSecurityKeyOption> option)
    {
        _option = option;
    }

    public string Get()
    {
        return _option.Value.Value;
    }
}
