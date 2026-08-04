namespace InvenTrack.Domain.Entities;

using InvenTrack.Domain.Common;

public class Supplier : BaseEntity
{
    public string CompanyName { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
