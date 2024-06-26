namespace ODour.Application.Feature.Admin.Order.SwitchOrderStatusToProcessing;

/// <summary>
///     Extension method for switch order status to processing feature.
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
    public static string ToAppCode(this SwitchOrderStatusToProcessingResponseStatusCode statusCode)
    {
        return $"{nameof(Admin)}.{nameof(Order)}.{nameof(SwitchOrderStatusToProcessing)}.{statusCode}";
    }
}
