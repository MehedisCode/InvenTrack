namespace InvenTrack.Application.Common.Interfaces;

using InvenTrack.Domain.Entities;

public interface ICategoryRepository : IRepository<Category>
{
    Task<IReadOnlyList<Category>> SearchAsync(string term, CancellationToken cancellationToken = default);
}
