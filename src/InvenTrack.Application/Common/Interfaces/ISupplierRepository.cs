namespace InvenTrack.Application.Common.Interfaces;

using InvenTrack.Application.Common.Models;
using InvenTrack.Domain.Entities;

public class SupplierQueryParameters : SortQuery
{
    public string? SearchTerm { get; set; }
}

public interface ISupplierRepository : IRepository<Supplier>
{
    Task<Supplier?> GetByIdWithProductsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Supplier>> SearchAsync(string term, CancellationToken cancellationToken = default);

    Task<PaginatedList<Supplier>> GetAllSuppliersAsync(SupplierQueryParameters parameters, CancellationToken cancellationToken = default);
}
