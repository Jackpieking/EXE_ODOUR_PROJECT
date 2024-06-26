using System;
using System.Text.Json.Serialization;
using ODour.Application.Feature.Guest.Cart.SyncGuestCartToUserCart;

namespace ODour.FastEndpointApi.Feature.Guest.Cart.SyncGuestCartToUserCart.HttpResponse;

internal sealed class SyncGuestCartToUserCartHttpResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int HttpCode { get; set; }

    public string AppCode { get; init; } =
        SyncGuestCartToUserCartResponseStatusCode.OPERATION_SUCCESS.ToAppCode();

    public DateTime ResponseTime { get; init; } =
        TimeZoneInfo.ConvertTimeFromUtc(
            dateTime: DateTime.UtcNow,
            destinationTimeZone: TimeZoneInfo.FindSystemTimeZoneById(id: "SE Asia Standard Time")
        );

    public object Body { get; init; } = new();

    public object Errors { get; init; } = new();
}
