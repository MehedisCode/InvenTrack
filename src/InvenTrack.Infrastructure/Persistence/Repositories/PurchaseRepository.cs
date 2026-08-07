namespace InvenTrack.Infrastructure.Persistence.Repositories;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using InvenTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class PurchaseRepository : Repository<Purchase>, IPurchaseRepository
{
    public PurchaseRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public override async Task<Purchase?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Purchases
            .Include(p => p.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public override async Task<IReadOnlyList<Purchase>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Purchases
            .AsNoTracking()
            .Include(p => p.Items)
            .ToListAsync(cancellationToken);
    }

    public async Task<PaginatedList<Purchase>> GetAllPurchasesAsync(PurchaseQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        var query = Context.Purchases
            .AsNoTracking()
            .Include(p => p.Items)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            query = query.Where(p => EF.Functions.ILike(p.PurchaseNumber, $"%{parameters.SearchTerm}%"));
        }

        query = parameters.SortBy?.ToLower() switch
        {
            "totalcost" => parameters.SortDescending ? query.OrderByDescending(p => p.TotalCost) : query.OrderBy(p => p.TotalCost),
            "purchasedate" => parameters.SortDescending ? query.OrderByDescending(p => p.PurchaseDate) : query.OrderBy(p => p.PurchaseDate),
            _ => query.OrderByDescending(p => p.CreatedAt)
        };

        var count = await query.CountAsync(cancellationToken);
        var items = await query.Skip((parameters.PageNumber - 1) * parameters.PageSize)
                               .Take(parameters.PageSize)
                               .ToListAsync(cancellationToken);

        return new PaginatedList<Purchase>(items, count, parameters.PageNumber, parameters.PageSize);
    }
}
