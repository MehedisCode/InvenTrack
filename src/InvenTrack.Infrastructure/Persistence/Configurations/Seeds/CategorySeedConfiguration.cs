namespace InvenTrack.Infrastructure.Persistence.Configurations.Seeds;

using System;
using InvenTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CategorySeedConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        var category1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var category2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");

        builder.HasData(
            new Category
            {
                Id = category1Id,
                Name = "Electronics",
                Description = "Electronic devices and accessories",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Category
            {
                Id = category2Id,
                Name = "Office Supplies",
                Description = "Stationery and office equipment",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
