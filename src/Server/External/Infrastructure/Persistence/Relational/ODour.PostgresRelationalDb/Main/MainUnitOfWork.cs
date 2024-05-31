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
    private readonly Lazy<DbContext> _context;

    public MainUnitOfWork(Lazy<DbContext> context)
    {
        _context = context;
    }

    public IRegisterAsUserRepository RegisterAsUserRepository
    {
        get
        {
            _registerAsUserRepository ??= new RegisterAsUserRepository(context: _context);

            return _registerAsUserRepository;
        }
    }

    public IRegisterAsAdminRepository RegisterAsAdminRepository
    {
        get
        {
            _registerAsAdminRepository ??= new RegisterAsAdminRepository(context: _context);

            return _registerAsAdminRepository;
        }
    }
}
