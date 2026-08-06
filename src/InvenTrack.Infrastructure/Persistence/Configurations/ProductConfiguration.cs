namespace InvenTrack.Infrastructure.Persistence.Configurations;

using InvenTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasIndex(p => p.SKU)
            .IsUnique();

        builder.Property(p => p.PurchasePrice)
            .HasPrecision(18, 2);

        builder.Property(p => p.SellingPrice)
            .HasPrecision(18, 2);

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Suppliers)
            .WithMany(s => s.Products)
            .UsingEntity(j => j.ToTable("SupplierProducts"));
    }
}