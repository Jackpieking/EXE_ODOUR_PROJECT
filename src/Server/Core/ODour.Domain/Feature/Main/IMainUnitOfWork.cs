using ODour.Domain.Feature.Main.Repository.Auth;
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
    #endregion
}
