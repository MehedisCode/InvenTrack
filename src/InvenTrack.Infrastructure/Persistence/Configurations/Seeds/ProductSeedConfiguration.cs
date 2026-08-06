namespace InvenTrack.Infrastructure.Persistence.Configurations.Seeds;

using System;
using InvenTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProductSeedConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        var category1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var category2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");

        builder.HasData(
            new Product
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444441"),
                Name = "Wireless Mouse",
                SKU = "WM-001",
                Description = "Ergonomic wireless mouse",
                PurchasePrice = 15.00m,
                SellingPrice = 25.99m,
                QuantityInStock = 50,
                CategoryId = category1Id,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Product
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444442"),
                Name = "Mechanical Keyboard",
                SKU = "MK-002",
                Description = "RGB Mechanical Keyboard",
                PurchasePrice = 55.00m,
                SellingPrice = 89.99m,
                QuantityInStock = 20,
                CategoryId = category1Id,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Product
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444443"),
                Name = "A4 Printer Paper",
                SKU = "PP-A4",
                Description = "500 sheets of A4 paper",
                PurchasePrice = 3.00m,
                SellingPrice = 5.49m,
                QuantityInStock = 200,
                CategoryId = category2Id,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
