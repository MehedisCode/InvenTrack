namespace InvenTrack.Infrastructure.Persistence.Configurations;

using InvenTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class StockTransactionConfiguration : IEntityTypeConfiguration<StockTransaction>
{
    public void Configure(EntityTypeBuilder<StockTransaction> builder)
    {
        builder.HasOne(st => st.Product)
            .WithMany(p => p.StockTransactions)
            .HasForeignKey(st => st.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(st => st.User)
            .WithMany(u => u.StockTransactions)
            .HasForeignKey(st => st.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(st => st.Sale)
            .WithMany(s => s.StockTransactions)
            .HasForeignKey(st => st.SaleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(st => st.Purchase)
            .WithMany(p => p.StockTransactions)
            .HasForeignKey(st => st.PurchaseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
