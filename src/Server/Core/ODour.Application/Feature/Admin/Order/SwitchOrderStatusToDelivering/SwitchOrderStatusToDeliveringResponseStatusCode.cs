namespace ODour.Application.Feature.Admin.Order.SwitchOrderStatusToDelivering;

public enum SwitchOrderStatusToDeliveringResponseStatusCode
{
    OPERATION_SUCCESS,
    FORBIDDEN,
    UN_AUTHORIZED,
    INPUT_VALIDATION_FAIL,
    OPERATION_FAIL,
    ORDER_NOT_FOUND,
}
