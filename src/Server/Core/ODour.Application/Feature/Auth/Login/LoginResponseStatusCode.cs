namespace ODour.Application.Feature.Auth.Login;

public enum LoginResponseStatusCode
{
    USER_IS_NOT_FOUND,
    USER_IS_TEMPORARILY_LOCKED_OUT,
    USER_IS_TEMPORARILY_BANNED,
    INPUT_VALIDATION_FAIL,
    OPERATION_SUCCESS,
    OPERATION_FAIL
}
