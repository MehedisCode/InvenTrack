namespace InvenTrack.Domain.UnitTests.Entities;

using FluentAssertions;
using InvenTrack.Domain.Entities;
using Xunit;

public class SupplierTests
{
    [Fact]
    public void Supplier_Should_HaveDefaultValues()
    {
        var supplier = new Supplier();

        supplier.Id.Should().NotBe(Guid.Empty);
        supplier.IsActive.Should().BeTrue();
        supplier.Purchases.Should().NotBeNull().And.BeEmpty();
        supplier.Products.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Supplier_Should_AllowProductsToBeLinked()
    {
        var supplier = new Supplier();

        supplier.Products.Add(new Product { Name = "Widget" });

        supplier.Products.Should().HaveCount(1);
    }
}
