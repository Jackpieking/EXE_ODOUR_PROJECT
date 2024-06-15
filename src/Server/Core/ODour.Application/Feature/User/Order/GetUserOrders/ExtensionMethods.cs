namespace ODour.Application.Feature.User.Order.GetUserOrders;

/// <summary>
///     Extension method for get user orders feature.
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
    public static string ToAppCode(this GetUserOrdersResponseStatusCode statusCode)
    {
        return $"{nameof(User)}.{nameof(Order)}.{nameof(GetUserOrders)}.{statusCode}";
    }
}
