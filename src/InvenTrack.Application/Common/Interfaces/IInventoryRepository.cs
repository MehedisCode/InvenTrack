namespace InvenTrack.Application.Common.Interfaces;

using InvenTrack.Application.Common.Models;
using InvenTrack.Domain.Entities;

public class TransactionQueryParameters : SortQuery
{
    public Guid? ProductId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public interface IInventoryRepository
{
    Task<PaginatedList<StockTransaction>> GetTransactionsAsync(TransactionQueryParameters parameters, CancellationToken cancellationToken = default);
    Task<StockTransaction> AddTransactionAsync(StockTransaction transaction, CancellationToken cancellationToken = default);
}
