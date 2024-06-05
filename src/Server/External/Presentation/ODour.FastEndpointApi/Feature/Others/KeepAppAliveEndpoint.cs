using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;

namespace ODour.FastEndpointApi.Feature.Others;

internal sealed class KeepAppAliveEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get(routePatterns: "app/check");
        AllowAnonymous();
    }

    public override Task HandleAsync(CancellationToken ct)
    {
        return SendOkAsync(response: "Successfully", cancellation: ct);
    }
}
