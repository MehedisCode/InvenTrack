namespace InvenTrack.Application.Common.Interfaces;

using InvenTrack.Domain.Entities;

public interface ISupplierRepository : IRepository<Supplier>
{
    Task<Supplier?> GetByIdWithProductsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Supplier>> SearchAsync(string term, CancellationToken cancellationToken = default);
}
