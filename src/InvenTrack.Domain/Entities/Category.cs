namespace InvenTrack.Domain.Entities;

using InvenTrack.Domain.Common;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
