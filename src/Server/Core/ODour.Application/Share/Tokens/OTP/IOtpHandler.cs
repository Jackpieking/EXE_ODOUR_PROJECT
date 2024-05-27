namespace ODour.Application.Share.Tokens.OTP;

/// <summary>
/// Interface for Otp Handler
/// </summary>
public interface IOtpHandler
{
    /// <summary>
    ///  Generate Otp
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    string Generate(int length);
}
