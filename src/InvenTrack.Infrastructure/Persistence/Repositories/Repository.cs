namespace InvenTrack.Infrastructure.Persistence.Repositories;

using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public abstract class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext Context;

    protected Repository(ApplicationDbContext context)
    {
        Context = context;
    }

    public virtual IQueryable<T> Query() => Context.Set<T>();

    public virtual Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Context.Set<T>().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async virtual Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = await Context.Set<T>().AsNoTracking().ToListAsync(cancellationToken);
        return (IReadOnlyList<T>)list;
    }

    public virtual Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        Context.Set<T>().Add(entity);
        return Task.FromResult(entity);
    }

    public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        Context.Set<T>().Update(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        Context.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }
}
