namespace InvenTrack.Domain.Entities;

using InvenTrack.Domain.Common;
using InvenTrack.Domain.Enums;

public class StockTransaction : BaseEntity
{
    public Guid ProductId { get; set; }
    public Guid UserId { get; set; }
    public int Quantity { get; set; }
    public TransactionType TransactionType { get; set; }
    public string? Remarks { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    public Product Product { get; set; } = null!;
    public User User { get; set; } = null!;
}
