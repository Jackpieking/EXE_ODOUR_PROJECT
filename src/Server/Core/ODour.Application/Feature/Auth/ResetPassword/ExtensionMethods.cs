namespace ODour.Application.Feature.Auth.ResetPassword;

/// <summary>
///     Extension method for reset password feature.
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
    public static string ToAppCode(this ResetPasswordResponseStatusCode statusCode)
    {
        return $"{nameof(Auth)}.{nameof(ResetPassword)}.{statusCode}";
    }
}
