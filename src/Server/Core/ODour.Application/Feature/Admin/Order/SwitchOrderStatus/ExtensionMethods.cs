namespace ODour.Application.Feature.Admin.Order.SwitchOrderStatus;

/// <summary>
///     Extension method for switch order status feature.
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
    public static string ToAppCode(this SwitchOrderStatusResponseStatusCode statusCode)
    {
        return $"{nameof(Admin)}.{nameof(Order)}.{nameof(SwitchOrderStatus)}.{statusCode}";
    }
}
