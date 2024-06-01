namespace ODour.Application.Feature.Auth.ResendUserConfirmationEmail;

/// <summary>
///     Extension method for resend user confirmation email feature.
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
    public static string ToAppCode(this ResendUserConfirmationEmailResponseStatusCode statusCode)
    {
        return $"{nameof(Auth)}.{nameof(ResendUserConfirmationEmail)}.{statusCode}";
    }
}
