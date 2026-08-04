namespace InvenTrack.Application.Common.Interfaces;

using InvenTrack.Application.Common.Models;
using InvenTrack.Domain.Entities;

public class ProductQueryParameters : SortQuery
{
    public string? SearchTerm { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? SupplierId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? LowStock { get; set; }
}

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaginatedList<Product>> GetProductsAsync(ProductQueryParameters parameters, CancellationToken cancellationToken = default);
    Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default);
    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
    Task DeleteAsync(Product product, CancellationToken cancellationToken = default);
}
