using System;

namespace ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatus.Common;

internal sealed class SwitchOrderStatusStateBag
{
    internal Guid CurrentOrderStatusId { get; set; }

    internal Guid OrderAuthorId { get; set; }
}
