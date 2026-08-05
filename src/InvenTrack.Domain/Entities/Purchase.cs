namespace InvenTrack.Domain.Entities;

using System;
using System.Collections.Generic;
using InvenTrack.Domain.Common;

public class Purchase : BaseEntity
{
    public string PurchaseNumber { get; set; } = string.Empty;
    public Guid SupplierId { get; set; }
    public Guid UserId { get; set; }
    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
    public decimal TotalCost { get; set; }

    public Supplier Supplier { get; set; } = null!;
    public User User { get; set; } = null!;
    public ICollection<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();
    public ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();
}
