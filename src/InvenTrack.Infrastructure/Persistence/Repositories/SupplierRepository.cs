namespace InvenTrack.Infrastructure.Persistence.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using InvenTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class SupplierRepository : Repository<Supplier>, ISupplierRepository
{
    public SupplierRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public override async Task<Supplier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == id && s.IsActive, cancellationToken);
    }

    public async Task<Supplier?> GetByIdWithProductsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Suppliers
            .Include(s => s.Products)
            .FirstOrDefaultAsync(s => s.Id == id && s.IsActive, cancellationToken);
    }

    public override async Task<IReadOnlyList<Supplier>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Suppliers
            .AsNoTracking()
            .Include(s => s.Products)
            .Where(s => s.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Supplier>> SearchAsync(string term, CancellationToken cancellationToken = default)
    {
        return await Context.Suppliers
            .AsNoTracking()
            .Where(s => s.IsActive &&
                        (EF.Functions.ILike(s.CompanyName, $"%{term}%") ||
                         EF.Functions.ILike(s.ContactPerson, $"%{term}%") ||
                         EF.Functions.ILike(s.Email, $"%{term}%")))
            .ToListAsync(cancellationToken);
    }

    public async Task<PaginatedList<Supplier>> GetAllSuppliersAsync(SupplierQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        var query = Context.Suppliers
            .AsNoTracking()
            .Include(s => s.Products)
            .Where(s => s.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            query = query.Where(s => EF.Functions.ILike(s.CompanyName, $"%{parameters.SearchTerm}%") ||
                                     EF.Functions.ILike(s.ContactPerson, $"%{parameters.SearchTerm}%") ||
                                     EF.Functions.ILike(s.Email, $"%{parameters.SearchTerm}%"));
        }

        query = parameters.SortBy?.ToLower() switch
        {
            "companyname" => parameters.SortDescending ? query.OrderByDescending(s => s.CompanyName) : query.OrderBy(s => s.CompanyName),
            _ => query.OrderByDescending(s => s.CreatedAt)
        };

        var count = await query.CountAsync(cancellationToken);
        var items = await query.Skip((parameters.PageNumber - 1) * parameters.PageSize)
                               .Take(parameters.PageSize)
                               .ToListAsync(cancellationToken);

        return new PaginatedList<Supplier>(items, count, parameters.PageNumber, parameters.PageSize);
    }
}
