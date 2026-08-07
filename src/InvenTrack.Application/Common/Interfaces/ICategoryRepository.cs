namespace InvenTrack.Application.Common.Interfaces;

using InvenTrack.Application.Common.Models;
using InvenTrack.Domain.Entities;

public class CategoryQueryParameters : SortQuery
{
    public string? SearchTerm { get; set; }
}

public interface ICategoryRepository : IRepository<Category>
{
    Task<IReadOnlyList<Category>> SearchAsync(string term, CancellationToken cancellationToken = default);

    Task<PaginatedList<Category>> GetAllCategoriesAsync(CategoryQueryParameters parameters, CancellationToken cancellationToken = default);
}
