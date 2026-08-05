namespace InvenTrack.Application.Features.Purchases.DTOs;

using System;
using System.Collections.Generic;

public class PurchaseDto
{
    public Guid Id { get; set; }
    public string PurchaseNumber { get; set; } = string.Empty;
    public Guid SupplierId { get; set; }
    public Guid UserId { get; set; }
    public DateTime PurchaseDate { get; set; }
    public decimal TotalCost { get; set; }
    public List<PurchaseItemDto> Items { get; set; } = new();
}

public class PurchaseItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal SubTotal { get; set; }
}
