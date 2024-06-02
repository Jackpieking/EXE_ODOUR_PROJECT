namespace ODour.Application.Share.DataProtection;

public interface IDataProtectionHandler
{
    /// <summary>
    ///     Encrypts the plain text.
    /// </summary>
    /// <param name="plaintext">
    ///     The plain text.
    /// </param>
    /// <returns>
    ///     The encrypted text.
    /// </returns>
    string Protect(string plaintext);

    /// <summary>
    ///     Unprotects the cipher text.
    /// </summary>
    /// <param name="ciphertext">
    ///     The cipher text.
    /// </param>
    /// <returns>
    ///     The plain text.
    /// </returns>
    string Unprotect(string ciphertext);
}
