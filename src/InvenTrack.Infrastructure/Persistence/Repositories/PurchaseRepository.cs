namespace InvenTrack.Infrastructure.Persistence.Repositories;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class PurchaseRepository : IPurchaseRepository
{
    private readonly ApplicationDbContext _context;

    public PurchaseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Purchase?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Purchases
            .Include(p => p.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Purchase>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Purchases
            .AsNoTracking()
            .Include(p => p.Items)
            .ToListAsync(cancellationToken);
    }

    public Task<Purchase> AddAsync(Purchase purchase, CancellationToken cancellationToken = default)
    {
        _context.Purchases.Add(purchase);
        return Task.FromResult(purchase);
    }
}
