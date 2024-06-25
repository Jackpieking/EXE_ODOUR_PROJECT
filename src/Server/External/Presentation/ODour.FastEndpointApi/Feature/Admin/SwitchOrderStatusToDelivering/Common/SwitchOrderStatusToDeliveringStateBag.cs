using System;

namespace ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToDelivering.Common;

internal sealed class SwitchOrderStatusToDeliveringStateBag
{
    internal Guid CurrentOrderStatusId { get; set; }

    internal Guid OrderAuthorId { get; set; }
}
