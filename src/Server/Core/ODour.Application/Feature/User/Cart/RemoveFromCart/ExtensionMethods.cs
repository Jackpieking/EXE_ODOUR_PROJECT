namespace ODour.Application.Feature.User.Cart.RemoveFromCart;

/// <summary>
///     Extension method for remove from cart feature.
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
    public static string ToAppCode(this RemoveFromCartResponseStatusCode statusCode)
    {
        return $"{nameof(User)}.{nameof(Cart)}.{nameof(RemoveFromCart)}.{statusCode}";
    }
}
