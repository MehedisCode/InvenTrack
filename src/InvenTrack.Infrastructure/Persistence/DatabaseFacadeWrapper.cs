namespace InvenTrack.Infrastructure.Persistence;

using System.Threading;
using System.Threading.Tasks;
using InvenTrack.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

public class DatabaseFacadeWrapper : IDatabaseFacade
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public DatabaseFacadeWrapper(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            return;
        }

        _transaction = await ((Microsoft.EntityFrameworkCore.DbContext)_context).Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}
