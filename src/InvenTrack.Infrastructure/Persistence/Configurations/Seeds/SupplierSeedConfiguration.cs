namespace InvenTrack.Infrastructure.Persistence.Configurations.Seeds;

using System;
using InvenTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class SupplierSeedConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        var supplier1Id = Guid.Parse("33333333-3333-3333-3333-333333333333");

        builder.HasData(
            new Supplier
            {
                Id = supplier1Id,
                CompanyName = "TechWholesale Inc.",
                ContactPerson = "John Doe",
                Email = "contact@techwholesale.com",
                Phone = "123-456-7890",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
