namespace ODour.Application.Feature.Admin.Order.SwitchOrderStatusToProcessing;

public enum SwitchOrderStatusToProcessingResponseStatusCode
{
    OPERATION_SUCCESS,
    FORBIDDEN,
    UN_AUTHORIZED,
    INPUT_VALIDATION_FAIL,
    OPERATION_FAIL,
    ORDER_NOT_FOUND,
}
