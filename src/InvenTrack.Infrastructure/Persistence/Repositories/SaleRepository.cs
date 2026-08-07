namespace InvenTrack.Infrastructure.Persistence.Repositories;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using InvenTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class SaleRepository : Repository<Sale>, ISaleRepository
{
    public SaleRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public override async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Sales
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public override async Task<IReadOnlyList<Sale>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Sales
            .AsNoTracking()
            .Include(s => s.Items)
            .ToListAsync(cancellationToken);
    }

    public async Task<PaginatedList<Sale>> GetAllSalesAsync(SaleQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        var query = Context.Sales
            .AsNoTracking()
            .Include(s => s.Items)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            query = query.Where(s => EF.Functions.ILike(s.SaleNumber, $"%{parameters.SearchTerm}%") ||
                                     EF.Functions.ILike(s.CustomerName, $"%{parameters.SearchTerm}%"));
        }

        query = parameters.SortBy?.ToLower() switch
        {
            "totalamount" => parameters.SortDescending ? query.OrderByDescending(s => s.TotalAmount) : query.OrderBy(s => s.TotalAmount),
            "saledate" => parameters.SortDescending ? query.OrderByDescending(s => s.SaleDate) : query.OrderBy(s => s.SaleDate),
            _ => query.OrderByDescending(s => s.CreatedAt)
        };

        var count = await query.CountAsync(cancellationToken);
        var items = await query.Skip((parameters.PageNumber - 1) * parameters.PageSize)
                               .Take(parameters.PageSize)
                               .ToListAsync(cancellationToken);

        return new PaginatedList<Sale>(items, count, parameters.PageNumber, parameters.PageSize);
    }
}
