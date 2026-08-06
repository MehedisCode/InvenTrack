namespace InvenTrack.Application.Features.Inventory.DTOs;

using System;

public class StockTransactionDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string Remarks { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
}
