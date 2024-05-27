using FastEndpoints;

namespace ODour.Application.Share.Features;

public interface IFeatureRequest<out TResponse> : ICommand<TResponse>
    where TResponse : class, IFeatureResponse { }
