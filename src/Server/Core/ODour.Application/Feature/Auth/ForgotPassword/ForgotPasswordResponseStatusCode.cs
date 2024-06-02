namespace ODour.Application.Feature.Auth.ForgotPassword;

public enum ForgotPasswordResponseStatusCode
{
    USER_IS_NOT_FOUND,
    INPUT_VALIDATION_FAIL,
    OPERATION_SUCCESS,
    OPERATION_FAIL,
    USER_IS_TEMPORARILY_BANNED
}
