namespace ODour.Application.Feature.Admin.Order.SwitchOrderStatusToDeliveringSuccessfully;

public enum SwitchOrderStatusToDeliveringSuccessfullyResponseStatusCode
{
    OPERATION_SUCCESS,
    FORBIDDEN,
    UN_AUTHORIZED,
    INPUT_VALIDATION_FAIL,
    OPERATION_FAIL,
    ORDER_NOT_FOUND,
}
