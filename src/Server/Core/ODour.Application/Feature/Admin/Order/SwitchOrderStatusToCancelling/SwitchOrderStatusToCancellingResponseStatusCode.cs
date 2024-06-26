namespace ODour.Application.Feature.Admin.Order.SwitchOrderStatusToCancelling;

public enum SwitchOrderStatusToCancellingResponseStatusCode
{
    OPERATION_SUCCESS,
    FORBIDDEN,
    UN_AUTHORIZED,
    INPUT_VALIDATION_FAIL,
    OPERATION_FAIL,
    ORDER_NOT_FOUND,
}
