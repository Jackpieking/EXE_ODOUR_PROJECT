using System.Threading;
using System.Threading.Tasks;

namespace ODour.Application.Share.Session;

public interface IUserSession
{
    Task SetAsync<TSource>(string key, TSource value, CancellationToken ct);

    Task<AppSessionModel<TSource>> GetAsync<TSource>(string key, CancellationToken ct);

    Task RemoveAsync(string key, CancellationToken ct);
}
