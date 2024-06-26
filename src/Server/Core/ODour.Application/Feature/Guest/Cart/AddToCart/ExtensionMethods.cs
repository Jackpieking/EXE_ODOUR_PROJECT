namespace ODour.Application.Feature.Guest.Cart.AddToCart;

/// <summary>
///     Extension method for guest add to cart feature.
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
    public static string ToAppCode(this GuestAddToCartResponseStatusCode statusCode)
    {
        return $"{nameof(Guest)}.{nameof(Cart)}.{nameof(AddToCart)}.{statusCode}";
    }
}
