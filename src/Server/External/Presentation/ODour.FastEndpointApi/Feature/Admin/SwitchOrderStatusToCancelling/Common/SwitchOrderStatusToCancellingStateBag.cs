using System;

namespace ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToCancelling.Common;

internal sealed class SwitchOrderStatusToCancellingStateBag
{
    internal Guid CurrentOrderStatusId { get; set; }

    internal Guid OrderAuthorId { get; set; }
}
