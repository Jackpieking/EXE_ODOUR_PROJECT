namespace ODour.Application.Feature.User.Order.CreateNewOrder;

/// <summary>
///     Extension method for create new user order feature.
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
    public static string ToAppCode(this CreateNewOrderResponseStatusCode statusCode)
    {
        return $"{nameof(User)}.{nameof(Order)}.{nameof(CreateNewOrder)}.{statusCode}";
    }
}
