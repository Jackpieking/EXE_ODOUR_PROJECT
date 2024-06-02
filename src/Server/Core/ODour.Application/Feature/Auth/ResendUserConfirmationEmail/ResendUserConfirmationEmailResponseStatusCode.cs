namespace ODour.Application.Feature.Auth.ResendUserConfirmationEmail;

public enum ResendUserConfirmationEmailResponseStatusCode
{
    USER_IS_NOT_FOUND,
    USER_HAS_CONFIRMED_EMAIL,
    INPUT_VALIDATION_FAIL,
    OPERATION_SUCCESS,
    OPERATION_FAIL,
    USER_IS_TEMPORARILY_BANNED
}
