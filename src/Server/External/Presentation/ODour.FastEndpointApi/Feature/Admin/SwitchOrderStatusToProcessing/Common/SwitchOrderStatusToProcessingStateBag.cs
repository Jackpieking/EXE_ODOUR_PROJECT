using System;

namespace ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToProcessing.Common;

internal sealed class SwitchOrderStatusToProcessingStateBag
{
    internal Guid CurrentOrderStatusId { get; set; }

    internal Guid OrderAuthorId { get; set; }
}
