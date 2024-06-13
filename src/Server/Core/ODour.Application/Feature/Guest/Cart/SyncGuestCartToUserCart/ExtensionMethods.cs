namespace ODour.Application.Feature.Guest.Cart.SyncGuestCartToUserCart;

/// <summary>
///     Extension method for sync guest cart to user cart feature.
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
    public static string ToAppCode(this SyncGuestCartToUserCartResponseStatusCode statusCode)
    {
        return $"{nameof(Guest)}.{nameof(Cart)}.{nameof(SyncGuestCartToUserCart)}.{statusCode}";
    }
}
