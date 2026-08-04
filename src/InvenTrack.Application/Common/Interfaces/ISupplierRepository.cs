namespace InvenTrack.Application.Common.Interfaces;

using InvenTrack.Domain.Entities;

public interface ISupplierRepository
{
    Task<Supplier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Supplier>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Supplier>> SearchAsync(string term, CancellationToken cancellationToken = default);
    Task<Supplier> AddAsync(Supplier supplier, CancellationToken cancellationToken = default);
    Task UpdateAsync(Supplier supplier, CancellationToken cancellationToken = default);
    Task DeleteAsync(Supplier supplier, CancellationToken cancellationToken = default);
}
