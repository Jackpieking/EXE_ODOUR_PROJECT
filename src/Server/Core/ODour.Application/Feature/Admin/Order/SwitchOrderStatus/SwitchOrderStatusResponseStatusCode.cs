namespace ODour.Application.Feature.Admin.Order.SwitchOrderStatus;

public enum SwitchOrderStatusResponseStatusCode
{
    OPERATION_SUCCESS,
    FORBIDDEN,
    UN_AUTHORIZED,
    INPUT_VALIDATION_FAIL,
    OPERATION_FAIL,
    ORDER_NOT_FOUND,
    ORDER_STATUS_NOT_FOUND
}
