namespace ODour.Application.Feature.Auth.RegisterAsAdmin;

/// <summary>
///     Extension method for register as admin feature.
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
    public static string ToAppCode(this RegisterAsAdminResponseStatusCode statusCode)
    {
        return $"{nameof(Auth)}.{nameof(RegisterAsAdmin)}.{statusCode}";
    }
}
