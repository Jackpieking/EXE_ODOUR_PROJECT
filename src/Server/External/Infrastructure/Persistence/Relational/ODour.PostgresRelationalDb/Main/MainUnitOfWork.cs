using System;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main;
using ODour.Domain.Feature.Main.Repository.Auth;
using ODour.Domain.Feature.Main.Repository.User.Product;
using ODour.PostgresRelationalDb.Main.Auth;
using ODour.PostgresRelationalDb.Main.User.Product;

namespace ODour.PostgresRelationalDb.Main;

public sealed class MainUnitOfWork : IMainUnitOfWork
{
    private IRegisterAsUserRepository _registerAsUserRepository;
    private IResendUserConfirmationEmailRepository _resendUserConfirmationEmailRepository;
    private IConfirmUserEmailRepository _confirmUserEmailRepository;
    private IForgotPasswordRepository _forgotPasswordRepository;
    private IResetPasswordRepository _resetPasswordRepository;
    private ILoginRepository _loginRepository;
    private ILogoutRepository _logoutRepository;
    private IRefreshAccessTokenRepository _refreshAccessTokenRepository;
    private IGetAllProductsRepository _getAllProductsRepository;
    private IGetProductDetailByProductIdRepository _getProductDetailByProductIdRepository;
    private IGetRelatedProductsByCategoryIdRepository _getRelatedProductsByCategoryIdRepository;
    private IGetProductsForHomePageRepository _getProductsForHomePageRepository;
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

    public ILogoutRepository LogoutRepository
    {
        get { return _logoutRepository ??= new LogoutRepository(context: _context); }
    }

    public IRefreshAccessTokenRepository RefreshAccessTokenRepository
    {
        get
        {
            return _refreshAccessTokenRepository ??= new RefreshAccessTokenRepository(
                context: _context
            );
        }
    }

    public IGetAllProductsRepository GetAllProductsRepository
    {
        get
        {
            return _getAllProductsRepository ??= new GetAllProductsRepository(context: _context);
        }
    }

    public IGetProductDetailByProductIdRepository GetProductDetailByProductIdRepository
    {
        get
        {
            return _getProductDetailByProductIdRepository ??=
                new GetProductDetailByProductIdRepository(context: _context);
        }
    }

    public IGetRelatedProductsByCategoryIdRepository GetRelatedProductsByCategoryIdRepository
    {
        get
        {
            return _getRelatedProductsByCategoryIdRepository ??=
                new GetRelatedProductsByCategoryIdRepository(context: _context);
        }
    }

    public IGetProductsForHomePageRepository GetProductsForHomePageRepository
    {
        get
        {
            return _getProductsForHomePageRepository ??= new GetProductsForHomePageRepository(
                context: _context
            );
        }
    }
}
