namespace ODour.Application.Feature.Guest.Cart.GetCartDetail;

/// <summary>
///     Extension method for guest get cart detail feature.
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
    public static string ToAppCode(this GuestGetCartDetailResponseStatusCode statusCode)
    {
        return $"{nameof(Guest)}.{nameof(Cart)}.{nameof(GetCartDetail)}.{statusCode}";
    }
}
