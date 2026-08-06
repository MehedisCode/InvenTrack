namespace InvenTrack.Domain.Entities;

using System;
using InvenTrack.Domain.Common;

public class SaleItem : BaseEntity
{
    public Guid SaleId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal SubTotal { get; set; }
    public Sale Sale { get; set; } = null!;
    public Product Product { get; set; } = null!;
}