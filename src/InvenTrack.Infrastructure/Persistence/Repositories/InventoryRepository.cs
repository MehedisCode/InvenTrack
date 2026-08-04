namespace InvenTrack.Infrastructure.Persistence.Repositories;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using InvenTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class InventoryRepository : IInventoryRepository
{
    private readonly ApplicationDbContext _context;

    public InventoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<StockTransaction>> GetTransactionsAsync(TransactionQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        var query = _context.StockTransactions
            .Include(st => st.Product)
            .Include(st => st.User)
            .AsNoTracking()
            .AsQueryable();

        if (parameters.ProductId.HasValue)
        {
            query = query.Where(st => st.ProductId == parameters.ProductId.Value);
        }

        if (parameters.StartDate.HasValue)
        {
            query = query.Where(st => st.TransactionDate >= parameters.StartDate.Value);
        }

        if (parameters.EndDate.HasValue)
        {
            query = query.Where(st => st.TransactionDate <= parameters.EndDate.Value);
        }

        query = parameters.SortBy?.ToLower() switch
        {
            "date" => parameters.SortDescending ? query.OrderByDescending(st => st.TransactionDate) : query.OrderBy(st => st.TransactionDate),
            "quantity" => parameters.SortDescending ? query.OrderByDescending(st => st.Quantity) : query.OrderBy(st => st.Quantity),
            _ => query.OrderByDescending(st => st.TransactionDate)
        };

        var count = await query.CountAsync(cancellationToken);
        var items = await query.Skip((parameters.PageNumber - 1) * parameters.PageSize)
                               .Take(parameters.PageSize)
                               .ToListAsync(cancellationToken);

        return new PaginatedList<StockTransaction>(items, count, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<StockTransaction> AddTransactionAsync(StockTransaction transaction, CancellationToken cancellationToken = default)
    {
        _context.StockTransactions.Add(transaction);
        await _context.SaveChangesAsync(cancellationToken);
        return transaction;
    }
}
