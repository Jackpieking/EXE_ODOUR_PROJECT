namespace ODour.Application.Feature.Auth.ConfirmUserEmail;

public enum ConfirmUserEmailResponseStatusCode
{
    INPUT_VALIDATION_FAIL,
    OPERATION_SUCCESS,
    OPERATION_FAIL,
    INVALID_TOKEN,
    USER_IS_NOT_FOUND,
    USER_HAS_CONFIRMED_EMAIL,
    USER_IS_TEMPORARILY_REMOVED
}
