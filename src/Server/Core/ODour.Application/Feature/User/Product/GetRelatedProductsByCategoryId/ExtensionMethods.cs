namespace ODour.Application.Feature.User.Product.GetRelatedProductsByCategoryId;

/// <summary>
///     Extension method for get product related products by category id feature.
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
    public static string ToAppCode(this GetRelatedProductsByCategoryIdResponseStatusCode statusCode)
    {
        return $"{nameof(User)}.{nameof(Product)}.{nameof(GetRelatedProductsByCategoryId)}.{statusCode}";
    }
}
