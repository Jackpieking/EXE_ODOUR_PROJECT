using System;
using System.Text.Json.Serialization;
using ODour.Application.Feature.Guest.Cart.AddToCart;

namespace ODour.FastEndpointApi.Feature.Guest.Cart.AddToCart.HttpResponse;

internal sealed class GuestAddToCartHttpResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int HttpCode { get; set; }

    public string AppCode { get; init; } =
        GuestAddToCartResponseStatusCode.OPERATION_SUCCESS.ToAppCode();

    public DateTime ResponseTime { get; init; } =
        TimeZoneInfo.ConvertTimeFromUtc(
            dateTime: DateTime.UtcNow,
            destinationTimeZone: TimeZoneInfo.FindSystemTimeZoneById(id: "SE Asia Standard Time")
        );

    public object Body { get; init; } = new();
}
