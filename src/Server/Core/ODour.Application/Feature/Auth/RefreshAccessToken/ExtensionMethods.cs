namespace ODour.Application.Feature.Auth.RefreshAccessToken;

/// <summary>
///     Extension method for refresh token feature.
/// </summary>
public static class ExtensionMethods
{
    /// <summary>
    ///     Mapping from feature response status code to
    ///     app code.
    /// </summary>
    /// <param name="statusCode">
    ///     Feature response status code
    /// </param>
    /// <returns>
    ///     New app code.
    /// </returns>
    public static string ToAppCode(this RefreshAccessTokenResponseStatusCode statusCode)
    {
        return $"{nameof(Auth)}.{nameof(RefreshAccessToken)}.{statusCode}";
    }
}
