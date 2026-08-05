namespace InvenTrack.Application.Common.Interfaces;

using System.Threading;
using System.Threading.Tasks;

public interface IDatabaseFacade
{
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}

public interface IApplicationDbContext
{
    IDatabaseFacade Database { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
