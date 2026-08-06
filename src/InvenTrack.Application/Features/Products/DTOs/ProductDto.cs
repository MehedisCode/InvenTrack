namespace InvenTrack.Application.Features.Products.DTOs;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public decimal PurchasePrice { get; set; }
    public decimal SellingPrice { get; set; }
    public int QuantityInStock { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public List<string> Suppliers { get; set; } = new();
}
