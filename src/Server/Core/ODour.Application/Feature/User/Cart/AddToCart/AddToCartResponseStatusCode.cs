namespace ODour.Application.Feature.User.Cart.AddToCart;

public enum AddToCartResponseStatusCode
{
    OPERATION_SUCCESS,
    INPUT_VALIDATION_FAIL,
    UN_AUTHORIZED,
    FORBIDDEN,
    OPERATION_FAIL,
    CART_IS_FULL,
    CART_ITEM_QUANTITY_EXCEED
}
