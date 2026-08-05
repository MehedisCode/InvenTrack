namespace InvenTrack.Domain.Entities;

using InvenTrack.Domain.Common;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal CostPrice { get; set; } = 0;
    public int QuantityInStock { get; set; }
    public Guid CategoryId { get; set; }
    public bool IsActive { get; set; } = true;

    public Category Category { get; set; } = null!;
    public ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();
    public ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();
}
