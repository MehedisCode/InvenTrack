namespace InvenTrack.Domain.Entities;

using Microsoft.AspNetCore.Identity;

public class Role : IdentityRole<Guid>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
}
