namespace InvenTrack.Infrastructure.Persistence.Repositories;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class SupplierRepository : ISupplierRepository
{
    private readonly ApplicationDbContext _context;

    public SupplierRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Supplier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == id && s.IsActive, cancellationToken);
    }

    public async Task<Supplier?> GetByIdWithProductsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers
            .Include(s => s.Products)
            .FirstOrDefaultAsync(s => s.Id == id && s.IsActive, cancellationToken);
    }

    public async Task<IReadOnlyList<Supplier>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers
            .AsNoTracking()
            .Include(s => s.Products)
            .Where(s => s.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Supplier>> SearchAsync(string term, CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers
            .AsNoTracking()
            .Where(s => s.IsActive &&
                        (EF.Functions.ILike(s.CompanyName, $"%{term}%") ||
                         EF.Functions.ILike(s.ContactPerson, $"%{term}%") ||
                         EF.Functions.ILike(s.Email, $"%{term}%")))
            .ToListAsync(cancellationToken);
    }

    public Task<Supplier> AddAsync(Supplier supplier, CancellationToken cancellationToken = default)
    {
        _context.Suppliers.Add(supplier);
        return Task.FromResult(supplier);
    }

    public Task UpdateAsync(Supplier supplier, CancellationToken cancellationToken = default)
    {
        _context.Suppliers.Update(supplier);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Supplier supplier, CancellationToken cancellationToken = default)
    {
        _context.Suppliers.Remove(supplier);
        return Task.CompletedTask;
    }
}
