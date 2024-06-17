using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main;
using ODour.Domain.Feature.Main.Repository.Admin.Order;
using ODour.Domain.Feature.Main.Repository.Auth;
using ODour.Domain.Feature.Main.Repository.Guest.Cart;
using ODour.Domain.Feature.Main.Repository.User.Cart;
using ODour.Domain.Feature.Main.Repository.User.Order;
using ODour.Domain.Feature.Main.Repository.User.Product;
using ODour.Domain.Share.Role.Entities;
using ODour.Domain.Share.User.Entities;
using ODour.PostgresRelationalDb.Main.Admin.Order;
using ODour.PostgresRelationalDb.Main.Auth;
using ODour.PostgresRelationalDb.Main.Guest.Cart;
using ODour.PostgresRelationalDb.Main.User.Cart;
using ODour.PostgresRelationalDb.Main.User.Order;
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
    private IGetCartDetailRepository _getCartDetailRepository;
    private IAddToCartRepository _addToCartRepository;
    private IRemoveFromCartRepository _removeFromCartRepository;
    private IGuestAddToCartRepository _guestAddToCartRepository;
    private IGuestGetCartDetailRepository _guestGetCartDetailRepository;
    private ISyncGuestCartToUserCartRepository _syncGuestCartToUserCartRepository;
    private IGetUserOrdersRepository _getUserOrdersRepository;
    private ICreateNewOrderRepository _createNewOrderRepository;
    private IGetOrderDetailRepository _getOrderDetailRepository;
    private ISwitchOrderStatusRepository _switchOrderStatusRepository;
    private ISwitchOrderStatusToProcessingRepository _switchOrderStatusToProcessingRepository;

    #region Dependencies
    private readonly Lazy<DbContext> _context;
    private readonly Lazy<UserManager<UserEntity>> _userManager;
    private readonly Lazy<SignInManager<UserEntity>> _signInManager;
    private readonly Lazy<RoleManager<RoleEntity>> _roleManager;
    #endregion

    public MainUnitOfWork(
        Lazy<DbContext> context,
        Lazy<UserManager<UserEntity>> userManager,
        Lazy<SignInManager<UserEntity>> signInManager,
        Lazy<RoleManager<RoleEntity>> roleManager
    )
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
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

    public IGetCartDetailRepository GetCartDetailRepository
    {
        get { return _getCartDetailRepository ??= new GetCartDetailRepository(context: _context); }
    }

    public IAddToCartRepository AddToCartRepository
    {
        get { return _addToCartRepository ??= new AddToCartRepository(context: _context); }
    }

    public IRemoveFromCartRepository RemoveFromCartRepository
    {
        get
        {
            return _removeFromCartRepository ??= new RemoveFromCartRepository(context: _context);
        }
    }

    public IGuestAddToCartRepository GuestAddToCartRepository
    {
        get
        {
            return _guestAddToCartRepository ??= new GuestAddToCartRepository(context: _context);
        }
    }

    public IGuestGetCartDetailRepository GuestGetCartDetailRepository
    {
        get
        {
            return _guestGetCartDetailRepository ??= new GuestGetCartDetailRepository(
                context: _context
            );
        }
    }

    public ISyncGuestCartToUserCartRepository SyncGuestCartToUserCartRepository
    {
        get
        {
            return _syncGuestCartToUserCartRepository ??= new SyncGuestCartToUserCartRepository(
                context: _context
            );
        }
    }

    public IGetUserOrdersRepository GetUserOrdersRepository
    {
        get { return _getUserOrdersRepository ??= new GetUserOrdersRepository(context: _context); }
    }

    public ICreateNewOrderRepository CreateNewOrderRepository
    {
        get
        {
            return _createNewOrderRepository ??= new CreateNewOrderRepository(context: _context);
        }
    }

    public IGetOrderDetailRepository GetOrderDetailRepository
    {
        get
        {
            return _getOrderDetailRepository ??= new GetOrderDetailRepository(context: _context);
        }
    }

    public ISwitchOrderStatusRepository SwitchOrderStatusRepository
    {
        get
        {
            return _switchOrderStatusRepository ??= new SwitchOrderStatusRepository(
                context: _context
            );
        }
    }

    public ISwitchOrderStatusToProcessingRepository SwitchOrderStatusToProcessingRepository
    {
        get
        {
            return _switchOrderStatusToProcessingRepository ??=
                new SwitchOrderStatusToProcessingRepository(context: _context);
        }
    }
}
