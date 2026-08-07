namespace InvenTrack.Application.Common.Interfaces;

using InvenTrack.Application.Common.Models;
using InvenTrack.Domain.Entities;

public class PurchaseQueryParameters : SortQuery
{
    public string? SearchTerm { get; set; }
}

public interface IPurchaseRepository : IRepository<Purchase>
{
    Task<PaginatedList<Purchase>> GetAllPurchasesAsync(PurchaseQueryParameters parameters, CancellationToken cancellationToken = default);
}
