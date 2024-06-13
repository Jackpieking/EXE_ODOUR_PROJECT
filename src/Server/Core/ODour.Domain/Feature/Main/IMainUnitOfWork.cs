using ODour.Domain.Feature.Main.Repository.Auth;
using ODour.Domain.Feature.Main.Repository.Guest.Cart;
using ODour.Domain.Feature.Main.Repository.User.Cart;
using ODour.Domain.Feature.Main.Repository.User.Product;

namespace ODour.Domain.Feature.Main;

public interface IMainUnitOfWork
{
    #region Auth
    IRegisterAsUserRepository RegisterAsUserRepository { get; }

    IResendUserConfirmationEmailRepository ResendUserConfirmationEmailRepository { get; }

    IConfirmUserEmailRepository ConfirmUserEmailRepository { get; }

    IForgotPasswordRepository ForgotPasswordRepository { get; }

    IResetPasswordRepository ResetPasswordRepository { get; }

    ILoginRepository LoginRepository { get; }

    ILogoutRepository LogoutRepository { get; }

    IRefreshAccessTokenRepository RefreshAccessTokenRepository { get; }
    #endregion

    #region User.Product
    IGetAllProductsRepository GetAllProductsRepository { get; }

    IGetProductDetailByProductIdRepository GetProductDetailByProductIdRepository { get; }

    IGetRelatedProductsByCategoryIdRepository GetRelatedProductsByCategoryIdRepository { get; }

    IGetProductsForHomePageRepository GetProductsForHomePageRepository { get; }
    #endregion

    #region User.Cart
    IGetCartDetailRepository GetCartDetailRepository { get; }

    IAddToCartRepository AddToCartRepository { get; }

    IRemoveFromCartRepository RemoveFromCartRepository { get; }
    #endregion

    #region Guest.Cart
    IGuestAddToCartRepository GuestAddToCartRepository { get; }

    IGuestGetCartDetailRepository GuestGetCartDetailRepository { get; }

    ISyncGuestCartToUserCartRepository SyncGuestCartToUserCartRepository { get; }
    #endregion
}
