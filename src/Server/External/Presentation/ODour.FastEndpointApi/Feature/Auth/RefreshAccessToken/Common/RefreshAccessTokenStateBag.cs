using System;

namespace ODour.FastEndpointApi.Feature.Auth.RefreshAccessToken.Common;

internal sealed class RefreshAccessTokenStateBag
{
    internal Guid FoundUserId { get; set; }

    internal Guid FoundAccessTokenId { get; set; }
}
