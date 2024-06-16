using System;
using System.Text.Json.Serialization;
using ODour.Application.Feature.User.Order.CreateNewOrder;

namespace ODour.FastEndpointApi.Feature.User.Order.CreateNewOrder.HttpResponse;

internal sealed class CreateNewOrderHttpResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int HttpCode { get; set; }

    public string AppCode { get; init; } =
        CreateNewOrderResponseStatusCode.OPERATION_SUCCESS.ToAppCode();

    public DateTime ResponseTime { get; init; } =
        TimeZoneInfo.ConvertTimeFromUtc(
            dateTime: DateTime.UtcNow,
            destinationTimeZone: TimeZoneInfo.FindSystemTimeZoneById(id: "SE Asia Standard Time")
        );

    public object Body { get; init; } = new();
}
