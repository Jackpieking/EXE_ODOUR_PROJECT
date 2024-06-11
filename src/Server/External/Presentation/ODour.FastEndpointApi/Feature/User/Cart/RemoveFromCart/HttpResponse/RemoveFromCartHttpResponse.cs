using System;
using System.Text.Json.Serialization;
using ODour.Application.Feature.User.Cart.RemoveFromCart;

namespace ODour.FastEndpointApi.Feature.User.Cart.RemoveFromCart.HttpResponse;

internal sealed class RemoveFromCartHttpResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int HttpCode { get; set; }

    public string AppCode { get; init; } =
        RemoveFromCartResponseStatusCode.OPERATION_SUCCESS.ToAppCode();

    public DateTime ResponseTime { get; init; } =
        TimeZoneInfo.ConvertTimeFromUtc(
            dateTime: DateTime.UtcNow,
            destinationTimeZone: TimeZoneInfo.FindSystemTimeZoneById(id: "SE Asia Standard Time")
        );

    public object Body { get; init; } = new();
}
