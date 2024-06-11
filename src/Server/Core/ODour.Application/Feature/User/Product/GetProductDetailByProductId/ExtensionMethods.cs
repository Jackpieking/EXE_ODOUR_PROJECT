namespace ODour.Application.Feature.User.Product.GetProductDetailByProductId;

/// <summary>
///     Extension method for get product detail by product id feature.
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
    public static string ToAppCode(this GetProductDetailByProductIdResponseStatusCode statusCode)
    {
        return $"{nameof(User)}.{nameof(Product)}.{nameof(GetProductDetailByProductId)}.{statusCode}";
    }
}
