namespace InvenTrack.Domain.Entities;

using System;
using InvenTrack.Domain.Common;
using InvenTrack.Domain.Enums;

public class StockTransaction : BaseEntity
{
    public Guid ProductId { get; set; }
    public Guid UserId { get; set; }
    public int Quantity { get; set; }
    public TransactionType TransactionType { get; set; }
    public Guid? SaleId { get; set; }
    public Guid? PurchaseId { get; set; }
    public string Remarks { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    public Product Product { get; set; } = null!;
    public User User { get; set; } = null!;
    public Sale? Sale { get; set; }
    public Purchase? Purchase { get; set; }
}
