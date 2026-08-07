namespace InvenTrack.Infrastructure.Persistence.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InvenTrack.Application.Common.Interfaces;
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
}
