namespace ODour.Application.Feature.Auth.ForgotPassword;

/// <summary>
///     Extension method for forgot password feature.
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
    public static string ToAppCode(this ForgotPasswordResponseStatusCode statusCode)
    {
        return $"{nameof(Auth)}.{nameof(ForgotPassword)}.{statusCode}";
    }
}
