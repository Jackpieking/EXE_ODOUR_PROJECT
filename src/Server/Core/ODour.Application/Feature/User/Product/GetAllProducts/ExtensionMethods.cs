namespace ODour.Application.Feature.User.Product.GetAllProducts;

/// <summary>
///     Extension method for get all products feature.
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
    public static string ToAppCode(this GetAllProductsResponseStatusCode statusCode)
    {
        return $"{nameof(User)}.{nameof(Product)}.{nameof(GetAllProducts)}.{statusCode}";
    }
}
