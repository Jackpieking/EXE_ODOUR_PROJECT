using System;

namespace ODour.Configuration.Presentation.WebApi.CORS;

public sealed class CORSOption
{
    public string[] Origins { get; set; } = Array.Empty<string>();

    public string[] Headers { get; set; } = Array.Empty<string>();

    public string[] Methods { get; set; } = Array.Empty<string>();

    public bool AreCredentialsAllowed { get; set; }
}
