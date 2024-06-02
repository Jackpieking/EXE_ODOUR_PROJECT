using System;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main;
using ODour.Domain.Feature.Main.Repository.Auth;
using ODour.PostgresRelationalDb.Main.Auth;

namespace ODour.PostgresRelationalDb.Main;

internal sealed class MainUnitOfWork : IMainUnitOfWork
{
    private IRegisterAsUserRepository _registerAsUserRepository;
    private IRegisterAsAdminRepository _registerAsAdminRepository;
    private IResendUserConfirmationEmailRepository _resendUserConfirmationEmailRepository;
    private IConfirmUserEmailRepository _confirmUserEmailRepository;
    private IForgotPasswordRepository _forgotPasswordRepository;
    private IResetPasswordRepository _resetPasswordRepository;
    private ILoginRepository _loginRepository;
    private readonly Lazy<DbContext> _context;

    public MainUnitOfWork(Lazy<DbContext> context)
    {
        _context = context;
    }

    public IRegisterAsUserRepository RegisterAsUserRepository
    {
        get
        {
            return _registerAsUserRepository ??= new RegisterAsUserRepository(context: _context);
        }
    }

    public IRegisterAsAdminRepository RegisterAsAdminRepository
    {
        get
        {
            return _registerAsAdminRepository ??= new RegisterAsAdminRepository(context: _context);
        }
    }

    public IResendUserConfirmationEmailRepository ResendUserConfirmationEmailRepository
    {
        get
        {
            return _resendUserConfirmationEmailRepository ??=
                new ResendUserConfirmationEmailRepository(context: _context);
        }
    }

    public IConfirmUserEmailRepository ConfirmUserEmailRepository
    {
        get
        {
            return _confirmUserEmailRepository ??= new ConfirmUserEmailRepository(
                context: _context
            );
        }
    }

    public IForgotPasswordRepository ForgotPasswordRepository
    {
        get
        {
            return _forgotPasswordRepository ??= new ForgotPasswordRepository(context: _context);
        }
    }

    public IResetPasswordRepository ResetPasswordRepository
    {
        get { return _resetPasswordRepository ??= new ResetPasswordRepository(context: _context); }
    }

    public ILoginRepository LoginRepository
    {
        get { return _loginRepository ??= new LoginRepository(context: _context); }
    }
}
