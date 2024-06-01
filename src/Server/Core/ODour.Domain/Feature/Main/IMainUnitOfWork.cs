using ODour.Domain.Feature.Main.Repository.Auth;

namespace ODour.Domain.Feature.Main;

public interface IMainUnitOfWork
{
    IRegisterAsUserRepository RegisterAsUserRepository { get; }

    IRegisterAsAdminRepository RegisterAsAdminRepository { get; }

    IResendUserConfirmationEmailRepository ResendUserConfirmationEmailRepository { get; }
}
