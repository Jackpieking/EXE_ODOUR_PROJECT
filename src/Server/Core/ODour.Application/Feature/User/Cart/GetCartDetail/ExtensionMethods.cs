namespace ODour.Application.Feature.User.Cart.GetCartDetail;

/// <summary>
///     Extension method for get cart detail feature.
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
    public static string ToAppCode(this GetCartDetailResponseStatusCode statusCode)
    {
        return $"{nameof(User)}.{nameof(Cart)}.{nameof(GetCartDetail)}.{statusCode}";
    }
}
