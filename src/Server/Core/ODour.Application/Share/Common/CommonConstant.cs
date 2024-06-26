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

        public const string DefaultAvatarUrl =
            "https://t4.ftcdn.net/jpg/05/49/98/39/360_F_549983970_bRCkYfk0P6PP5fKbMhZMIb07mCJ6esXL.jpg";

        public static readonly JsonSerializerOptions DefaultJsonSerializerOptions =
            new()
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

        public const string DefaultStringSeparator = "<APP_TK>";

        public const string AppCartSessionKey = "APP_CART_SESSION_KEY";
    }
}
