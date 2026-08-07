namespace InvenTrack.Application.Common.Interfaces;

using InvenTrack.Application.Common.Models;
using InvenTrack.Domain.Entities;

public class SaleQueryParameters : SortQuery
{
    public string? SearchTerm { get; set; }
}

public interface ISaleRepository : IRepository<Sale>
{
    Task<PaginatedList<Sale>> GetAllSalesAsync(SaleQueryParameters parameters, CancellationToken cancellationToken = default);
}
