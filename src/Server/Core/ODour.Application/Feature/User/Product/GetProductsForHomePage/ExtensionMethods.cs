namespace ODour.Application.Feature.User.Product.GetProductsForHomePage;

/// <summary>
///     Extension method for get products for home page feature.
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
    public static string ToAppCode(this GetProductsForHomePageResponseStatusCode statusCode)
    {
        return $"{nameof(User)}.{nameof(Product)}.{nameof(GetProductsForHomePage)}.{statusCode}";
    }
}
