namespace ODour.Application.Feature.Admin.Order.SwitchOrderStatusToCancelling;

/// <summary>
///     Extension method for switch order status to cancelling feature.
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
    public static string ToAppCode(this SwitchOrderStatusToCancellingResponseStatusCode statusCode)
    {
        return $"{nameof(Admin)}.{nameof(Order)}.{nameof(SwitchOrderStatusToCancelling)}.{statusCode}";
    }
}
