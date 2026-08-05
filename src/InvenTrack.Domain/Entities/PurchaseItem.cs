namespace InvenTrack.Domain.Entities;

using System;
using InvenTrack.Domain.Common;

public class PurchaseItem : BaseEntity
{
    public Guid PurchaseId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal SubTotal { get; set; }

    public Purchase Purchase { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
