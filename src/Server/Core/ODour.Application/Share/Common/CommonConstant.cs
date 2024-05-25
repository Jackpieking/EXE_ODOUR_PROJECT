using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ODour.Application.Share.Common;

public static class CommonConstant
{
    public static class App
    {
        public static readonly DateTime MinTimeInUTC = DateTime.MinValue.ToUniversalTime();

        public static readonly Guid DefaultGuidValue = Guid.Parse(
            input: "7b513726-d8a9-4849-b797-4e31a34c378f"
        );

        public static readonly JsonSerializerOptions DefaultJsonSerializerOptions =
            new()
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
    }
}
