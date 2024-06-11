namespace ODour.Application.Feature.User.Cart.AddToCart;

/// <summary>
///     Extension method for add to cart feature.
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
    public static string ToAppCode(this AddToCartResponseStatusCode statusCode)
    {
        return $"{nameof(User)}.{nameof(Cart)}.{nameof(AddToCart)}.{statusCode}";
    }
}
