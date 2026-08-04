namespace InvenTrack.Domain.Entities;

using InvenTrack.Domain.Common;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal UnitPrice { get; set; }
    public int QuantityInStock { get; set; }
    public Guid CategoryId { get; set; }
    public Guid SupplierId { get; set; }
    public bool IsActive { get; set; } = true;

    public Category Category { get; set; } = null!;
    public Supplier Supplier { get; set; } = null!;
    public ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();
}
