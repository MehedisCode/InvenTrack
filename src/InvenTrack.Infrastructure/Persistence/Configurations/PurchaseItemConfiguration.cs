namespace InvenTrack.Infrastructure.Persistence.Configurations;

using InvenTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PurchaseItemConfiguration : IEntityTypeConfiguration<PurchaseItem>
{
    public void Configure(EntityTypeBuilder<PurchaseItem> builder)
    {
        builder.Property(pi => pi.UnitCost).HasPrecision(18, 2);
        builder.Property(pi => pi.SubTotal).HasPrecision(18, 2);

        builder.HasOne(pi => pi.Purchase)
            .WithMany(p => p.Items)
            .HasForeignKey(pi => pi.PurchaseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pi => pi.Product)
            .WithMany()
            .HasForeignKey(pi => pi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
