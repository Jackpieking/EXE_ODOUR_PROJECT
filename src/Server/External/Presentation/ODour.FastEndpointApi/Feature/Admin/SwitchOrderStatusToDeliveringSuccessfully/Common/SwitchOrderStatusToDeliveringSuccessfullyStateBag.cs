using System;

namespace ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToDeliveringSuccessfully.Common;

internal sealed class SwitchOrderStatusToDeliveringSuccessfullyStateBag
{
    internal Guid CurrentOrderStatusId { get; set; }

    internal Guid OrderAuthorId { get; set; }
}
