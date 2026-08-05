namespace InvenTrack.Application.Features.Dashboard.DTOs;

using System;
using System.Collections.Generic;

public class DashboardDto
{
    public OverviewDto Overview { get; set; } = new();
    public FinancialsDto Financials { get; set; } = new();
    public RecentActivityDto RecentActivity { get; set; } = new();
    public List<TopSellingProductDto> TopSellingProducts { get; set; } = new();
    public List<LowStockAlertDto> LowStockAlerts { get; set; } = new();
    public List<SalesByCategoryDto> SalesByCategory { get; set; } = new();
    public List<StockMovementDto> StockMovementLast30Days { get; set; } = new();
}

public class OverviewDto
{
    public int TotalProducts { get; set; }
    public int ActiveProducts { get; set; }
    public int TotalCategories { get; set; }
    public int TotalSuppliers { get; set; }
    public int TotalStockUnits { get; set; }
    public int LowStockProducts { get; set; }
    public int OutOfStockProducts { get; set; }
}

public class FinancialsDto
{
    public decimal TotalInventoryValue { get; set; }
    public decimal TotalSalesRevenue { get; set; }
    public decimal TotalPurchaseCost { get; set; }
    public decimal EstimatedProfit { get; set; }
    public decimal TotalSalesThisMonth { get; set; }
    public decimal TotalPurchasesThisMonth { get; set; }
}

public class RecentActivityDto
{
    public int TotalSales { get; set; }
    public int TotalPurchases { get; set; }
    public int SalesLast7Days { get; set; }
    public int PurchasesLast7Days { get; set; }
}

public class TopSellingProductDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int TotalQuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class LowStockAlertDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int QuantityInStock { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}

public class SalesByCategoryDto
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal TotalRevenue { get; set; }
    public int TotalItemsSold { get; set; }
}

public class StockMovementDto
{
    public DateTime Date { get; set; }
    public int StockIn { get; set; }
    public int StockOut { get; set; }
}
