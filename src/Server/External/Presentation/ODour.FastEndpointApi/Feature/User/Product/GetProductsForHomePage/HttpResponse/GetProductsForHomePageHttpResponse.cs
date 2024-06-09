using System;
using System.Text.Json.Serialization;
using ODour.Application.Feature.User.Product.GetProductsForHomePage;

namespace ODour.FastEndpointApi.Feature.User.Product.GetProductsForHomePage.HttpResponse;

internal sealed class GetProductsForHomePageHttpResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int HttpCode { get; set; }

    public string AppCode { get; init; } =
        GetProductsForHomePageResponseStatusCode.OPERATION_SUCCESS.ToAppCode();

    public DateTime ResponseTime { get; init; } =
        TimeZoneInfo.ConvertTimeFromUtc(
            dateTime: DateTime.UtcNow,
            destinationTimeZone: TimeZoneInfo.FindSystemTimeZoneById(id: "SE Asia Standard Time")
        );

    public object Body { get; init; } = new();

    public object Errors { get; init; } = new();
}
