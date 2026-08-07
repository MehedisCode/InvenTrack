namespace InvenTrack.Application.Common.Interfaces;

using InvenTrack.Application.Common.Models;
using InvenTrack.Domain.Entities;

public class ProductQueryParameters : SortQuery
{
    public string? SearchTerm { get; set; }
    public Guid? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? LowStock { get; set; }
}

public interface IProductRepository : IRepository<Product>
{
    Task<PaginatedList<Product>> GetProductsAsync(ProductQueryParameters parameters, CancellationToken cancellationToken = default);
}
