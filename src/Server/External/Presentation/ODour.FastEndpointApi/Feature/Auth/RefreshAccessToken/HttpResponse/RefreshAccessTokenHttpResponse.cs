using System;
using System.Text.Json.Serialization;
using ODour.Application.Feature.Auth.RefreshAccessToken;

namespace ODour.FastEndpointApi.Feature.Auth.RefreshAccessToken.HttpResponse;

internal sealed class RefreshAccessTokenHttpResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int HttpCode { get; set; }

    public string AppCode { get; init; } =
        RefreshAccessTokenResponseStatusCode.OPERATION_SUCCESS.ToAppCode();

    public DateTime ResponseTime { get; init; } =
        TimeZoneInfo.ConvertTimeFromUtc(
            dateTime: DateTime.UtcNow,
            destinationTimeZone: TimeZoneInfo.FindSystemTimeZoneById(id: "SE Asia Standard Time")
        );

    public object Body { get; init; } = new();

    public object Errors { get; init; } = new();
}
