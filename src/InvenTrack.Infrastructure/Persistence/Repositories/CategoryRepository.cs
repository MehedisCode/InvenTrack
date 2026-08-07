namespace InvenTrack.Infrastructure.Persistence.Repositories;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InvenTrack.Application.Common.Interfaces;
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
}
