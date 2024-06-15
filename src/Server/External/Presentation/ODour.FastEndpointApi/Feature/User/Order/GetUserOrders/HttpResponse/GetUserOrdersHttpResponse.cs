using System;
using System.Text.Json.Serialization;
using ODour.Application.Feature.User.Order.GetUserOrders;

namespace ODour.FastEndpointApi.Feature.User.Order.GetUserOrders.HttpResponse;

internal sealed class GetUserOrdersHttpResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int HttpCode { get; set; }

    public string AppCode { get; init; } =
        GetUserOrdersResponseStatusCode.OPERATION_SUCCESS.ToAppCode();

    public DateTime ResponseTime { get; init; } =
        TimeZoneInfo.ConvertTimeFromUtc(
            dateTime: DateTime.UtcNow,
            destinationTimeZone: TimeZoneInfo.FindSystemTimeZoneById(id: "SE Asia Standard Time")
        );

    public object Body { get; init; } = new();
}
