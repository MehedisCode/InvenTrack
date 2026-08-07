namespace InvenTrack.Infrastructure.Persistence.Repositories;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using InvenTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<Category>> SearchAsync(string term, CancellationToken cancellationToken = default)
    {
        return await Context.Categories
            .AsNoTracking()
            .Where(c => EF.Functions.ILike(c.Name, $"%{term}%") || (c.Description != null && EF.Functions.ILike(c.Description, $"%{term}%")))
            .ToListAsync(cancellationToken);
    }

    public async Task<PaginatedList<Category>> GetAllCategoriesAsync(CategoryQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        var query = Context.Categories
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            query = query.Where(c => EF.Functions.ILike(c.Name, $"%{parameters.SearchTerm}%") ||
                                     (c.Description != null && EF.Functions.ILike(c.Description, $"%{parameters.SearchTerm}%")));
        }

        query = parameters.SortBy?.ToLower() switch
        {
            "name" => parameters.SortDescending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
            _ => query.OrderByDescending(c => c.CreatedAt)
        };

        var count = await query.CountAsync(cancellationToken);
        var items = await query.Skip((parameters.PageNumber - 1) * parameters.PageSize)
                               .Take(parameters.PageSize)
                               .ToListAsync(cancellationToken);

        return new PaginatedList<Category>(items, count, parameters.PageNumber, parameters.PageSize);
    }
}
