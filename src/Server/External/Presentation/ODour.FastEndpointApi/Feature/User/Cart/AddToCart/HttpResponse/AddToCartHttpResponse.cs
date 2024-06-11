using System;
using System.Text.Json.Serialization;
using ODour.Application.Feature.User.Cart.AddToCart;

namespace ODour.FastEndpointApi.Feature.User.Cart.AddToCart.HttpResponse;

internal sealed class AddToCartHttpResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int HttpCode { get; set; }

    public string AppCode { get; init; } =
        AddToCartResponseStatusCode.OPERATION_SUCCESS.ToAppCode();

    public DateTime ResponseTime { get; init; } =
        TimeZoneInfo.ConvertTimeFromUtc(
            dateTime: DateTime.UtcNow,
            destinationTimeZone: TimeZoneInfo.FindSystemTimeZoneById(id: "SE Asia Standard Time")
        );

    public object Body { get; init; } = new();
}
