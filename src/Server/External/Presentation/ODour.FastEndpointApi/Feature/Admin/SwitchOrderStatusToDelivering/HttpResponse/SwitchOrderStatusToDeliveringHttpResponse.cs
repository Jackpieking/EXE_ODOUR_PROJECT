﻿using System;
using System.Text.Json.Serialization;
using ODour.Application.Feature.Admin.Order.SwitchOrderStatusToDelivering;

namespace ODour.FastEndpointApi.Feature.Admin.SwitchOrderStatusToDelivering.HttpResponse;

internal sealed class SwitchOrderStatusToDeliveringHttpResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int HttpCode { get; set; }

    public string AppCode { get; init; } =
        SwitchOrderStatusToDeliveringResponseStatusCode.OPERATION_SUCCESS.ToAppCode();

    public DateTime ResponseTime { get; init; } =
        TimeZoneInfo.ConvertTimeFromUtc(
            dateTime: DateTime.UtcNow,
            destinationTimeZone: TimeZoneInfo.FindSystemTimeZoneById(id: "SE Asia Standard Time")
        );

    public object Body { get; init; } = new();
}
