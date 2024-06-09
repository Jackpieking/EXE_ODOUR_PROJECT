using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ODour.Application.Share.Common;
using ODour.Application.Share.Session;

namespace ODour.RedisSessionStorage.Handler;

public sealed class UserSessionHandler : IUserSession
{
    private readonly Lazy<IHttpContextAccessor> _httpContextAccessor;

    public UserSessionHandler(Lazy<IHttpContextAccessor> httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task RemoveAsync(string key, CancellationToken ct)
    {
        await _httpContextAccessor.Value.HttpContext.Session.LoadAsync(cancellationToken: ct);

        _httpContextAccessor.Value.HttpContext.Session.Remove(key: key);
    }

    public async Task AddAsync<TSource>(string key, TSource value, CancellationToken ct)
    {
        await _httpContextAccessor.Value.HttpContext.Session.LoadAsync(cancellationToken: ct);

        _httpContextAccessor.Value.HttpContext.Session.SetString(
            key: key,
            value: JsonSerializer.Serialize(
                value: value,
                options: CommonConstant.App.DefaultJsonSerializerOptions
            )
        );
    }

    public async Task<AppSessionModel<TSource>> GetAsync<TSource>(string key, CancellationToken ct)
    {
        await _httpContextAccessor.Value.HttpContext.Session.LoadAsync(cancellationToken: ct);

        var valueAsJson = _httpContextAccessor.Value.HttpContext.Session.GetString(key: key);

        if (string.IsNullOrWhiteSpace(value: valueAsJson))
        {
            return default;
        }

        return new()
        {
            Value = JsonSerializer.Deserialize<TSource>(
                json: valueAsJson,
                options: CommonConstant.App.DefaultJsonSerializerOptions
            )
        };
    }
}
