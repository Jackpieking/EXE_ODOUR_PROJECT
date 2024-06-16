namespace ODour.Application.Feature.User.Order.GetOrderDetail;

/// <summary>
///     Extension method for get order detail feature.
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
    public static string ToAppCode(this GetOrderDetailResponseStatusCode statusCode)
    {
        return $"{nameof(User)}.{nameof(Order)}.{nameof(GetOrderDetail)}.{statusCode}";
    }
}
