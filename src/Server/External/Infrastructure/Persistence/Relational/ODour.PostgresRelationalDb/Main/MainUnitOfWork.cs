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
}
