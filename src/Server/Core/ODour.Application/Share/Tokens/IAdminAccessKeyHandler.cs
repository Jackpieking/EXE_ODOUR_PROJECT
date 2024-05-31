namespace ODour.Application.Share.Tokens;

public interface IAdminAccessKeyHandler
{
    /// <summary>
    ///     Get admin access key.
    /// </summary>
    /// <returns>
    ///     Admin access key.
    /// </returns>
    string Get();
}
