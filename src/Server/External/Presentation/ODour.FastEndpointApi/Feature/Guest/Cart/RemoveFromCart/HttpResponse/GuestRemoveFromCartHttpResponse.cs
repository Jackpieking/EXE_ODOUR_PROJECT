using System;
using System.Text.Json.Serialization;
using ODour.Application.Feature.Guest.Cart.RemoveFromCart;

namespace ODour.FastEndpointApi.Feature.Guest.Cart.RemoveFromCart.HttpResponse;

internal sealed class GuestRemoveFromCartHttpResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int HttpCode { get; set; }

    public string AppCode { get; init; } =
        GuestRemoveFromCartResponseStatusCode.OPERATION_SUCCESS.ToAppCode();

    public DateTime ResponseTime { get; init; } =
        TimeZoneInfo.ConvertTimeFromUtc(
            dateTime: DateTime.UtcNow,
            destinationTimeZone: TimeZoneInfo.FindSystemTimeZoneById(id: "SE Asia Standard Time")
        );

    public object Body { get; init; } = new();
}
