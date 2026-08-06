namespace InvenTrack.Infrastructure.Persistence.Repositories;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Dashboard.DTOs;
using InvenTrack.Domain.Enums;
using Microsoft.EntityFrameworkCore;

public class DashboardRepository : IDashboardRepository
{
    private readonly ApplicationDbContext _context;
    private const int LowStockThreshold = 10;
    private const int TopSellingCount = 5;

    public DashboardRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardDto> GetDashboardStatsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var sevenDaysAgo = now.AddDays(-7);
        var thirtyDaysAgo = now.AddDays(-30);

        // ── Overview ─────────────────────────────────────────────────────────
        var totalProducts = await _context.Products.CountAsync(cancellationToken);
        var activeProducts = await _context.Products.CountAsync(p => p.IsActive, cancellationToken);
        var totalCategories = await _context.Categories.CountAsync(cancellationToken);
        var totalSuppliers = await _context.Suppliers.CountAsync(cancellationToken);
        var totalStockUnits = await _context.Products.SumAsync(p => p.QuantityInStock, cancellationToken);
        var lowStockProducts = await _context.Products
            .CountAsync(p => p.IsActive && p.QuantityInStock > 0 && p.QuantityInStock <= LowStockThreshold, cancellationToken);
        var outOfStockProducts = await _context.Products
            .CountAsync(p => p.IsActive && p.QuantityInStock == 0, cancellationToken);

        // ── Financials ───────────────────────────────────────────────────────
        var totalInventoryValue = await _context.Products
            .SumAsync(p => p.PurchasePrice * p.QuantityInStock, cancellationToken);
        var totalSalesRevenue = await _context.Sales
            .SumAsync(s => (decimal?)s.TotalAmount ?? 0, cancellationToken);
        var totalPurchaseCost = await _context.Purchases
            .SumAsync(p => (decimal?)p.TotalCost ?? 0, cancellationToken);
        var totalSalesThisMonth = await _context.Sales
            .Where(s => s.SaleDate >= startOfMonth)
            .SumAsync(s => (decimal?)s.TotalAmount ?? 0, cancellationToken);
        var totalPurchasesThisMonth = await _context.Purchases
            .Where(p => p.PurchaseDate >= startOfMonth)
            .SumAsync(p => (decimal?)p.TotalCost ?? 0, cancellationToken);

        // ── Recent Activity ──────────────────────────────────────────────────
        var totalSales = await _context.Sales.CountAsync(cancellationToken);
        var totalPurchases = await _context.Purchases.CountAsync(cancellationToken);
        var salesLast7Days = await _context.Sales
            .CountAsync(s => s.SaleDate >= sevenDaysAgo, cancellationToken);
        var purchasesLast7Days = await _context.Purchases
            .CountAsync(p => p.PurchaseDate >= sevenDaysAgo, cancellationToken);

        // ── Top 5 Selling Products ───────────────────────────────────────────
        var topSellingProducts = await _context.SaleItems
            .AsNoTracking()
            .GroupBy(si => new { si.ProductId, si.Product.Name, si.Product.SKU })
            .Select(g => new TopSellingProductDto
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.Name,
                SKU = g.Key.SKU,
                TotalQuantitySold = g.Sum(si => si.Quantity),
                TotalRevenue = g.Sum(si => si.SubTotal)
            })
            .OrderByDescending(x => x.TotalQuantitySold)
            .Take(TopSellingCount)
            .ToListAsync(cancellationToken);

        // ── Low Stock Alerts (≤ 10, active only) ────────────────────────────
        var lowStockAlerts = await _context.Products
            .AsNoTracking()
            .Where(p => p.IsActive && p.QuantityInStock <= LowStockThreshold)
            .OrderBy(p => p.QuantityInStock)
            .Select(p => new LowStockAlertDto
            {
                ProductId = p.Id,
                ProductName = p.Name,
                SKU = p.SKU,
                QuantityInStock = p.QuantityInStock,
                CategoryName = p.Category.Name
            })
            .ToListAsync(cancellationToken);

        // ── Sales by Category ────────────────────────────────────────────────
        var salesByCategory = await _context.SaleItems
            .AsNoTracking()
            .GroupBy(si => si.Product.Category.Name)
            .Select(g => new SalesByCategoryDto
            {
                CategoryName = g.Key,
                TotalRevenue = g.Sum(si => si.SubTotal),
                TotalItemsSold = g.Sum(si => si.Quantity)
            })
            .OrderByDescending(x => x.TotalRevenue)
            .ToListAsync(cancellationToken);

        // ── Stock Movement Last 30 Days ──────────────────────────────────────
        var rawMovements = await _context.StockTransactions
            .AsNoTracking()
            .Where(st => st.TransactionDate >= thirtyDaysAgo)
            .Select(st => new
            {
                Date = st.TransactionDate.Date,
                st.TransactionType,
                st.Quantity
            })
            .ToListAsync(cancellationToken);

        var stockMovement = rawMovements
            .GroupBy(st => st.Date)
            .Select(g => new StockMovementDto
            {
                Date = g.Key,
                StockIn = g.Where(x => x.TransactionType == TransactionType.StockIn).Sum(x => x.Quantity),
                StockOut = g.Where(x => x.TransactionType == TransactionType.StockOut).Sum(x => x.Quantity)
            })
            .OrderBy(x => x.Date)
            .ToList();

        // ── Assemble ─────────────────────────────────────────────────────────
        return new DashboardDto
        {
            Overview = new OverviewDto
            {
                TotalProducts = totalProducts,
                ActiveProducts = activeProducts,
                TotalCategories = totalCategories,
                TotalSuppliers = totalSuppliers,
                TotalStockUnits = totalStockUnits,
                LowStockProducts = lowStockProducts,
                OutOfStockProducts = outOfStockProducts
            },
            Financials = new FinancialsDto
            {
                TotalInventoryValue = totalInventoryValue,
                TotalSalesRevenue = totalSalesRevenue,
                TotalPurchaseCost = totalPurchaseCost,
                EstimatedProfit = totalSalesRevenue - totalPurchaseCost,
                TotalSalesThisMonth = totalSalesThisMonth,
                TotalPurchasesThisMonth = totalPurchasesThisMonth
            },
            RecentActivity = new RecentActivityDto
            {
                TotalSales = totalSales,
                TotalPurchases = totalPurchases,
                SalesLast7Days = salesLast7Days,
                PurchasesLast7Days = purchasesLast7Days
            },
            TopSellingProducts = topSellingProducts,
            LowStockAlerts = lowStockAlerts,
            SalesByCategory = salesByCategory,
            StockMovementLast30Days = stockMovement
        };
    }
}
