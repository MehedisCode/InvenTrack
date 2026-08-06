namespace InvenTrack.Infrastructure.Persistence.Configurations;

using InvenTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.Property(si => si.SellingPrice)
            .HasPrecision(18, 2);

        builder.Property(si => si.SubTotal)
            .HasPrecision(18, 2);

        builder.HasOne(si => si.Sale)
            .WithMany(s => s.Items)
            .HasForeignKey(si => si.SaleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(si => si.Product)
            .WithMany()
            .HasForeignKey(si => si.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}