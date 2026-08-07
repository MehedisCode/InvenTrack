namespace InvenTrack.Domain.UnitTests.Entities;

using FluentAssertions;
using InvenTrack.Domain.Entities;
using Xunit;

public class SaleTests
{
    [Fact]
    public void Sale_Should_HaveDefaultValues()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var sale = new Sale();
        var after = DateTime.UtcNow.AddSeconds(1);

        sale.Id.Should().NotBe(Guid.Empty);
        sale.Items.Should().NotBeNull().And.BeEmpty();
        sale.StockTransactions.Should().NotBeNull().And.BeEmpty();
        sale.SaleDate.Should().BeOnOrAfter(before);
        sale.SaleDate.Should().BeOnOrBefore(after);
        sale.TotalAmount.Should().Be(0);
    }

    [Fact]
    public void Sale_Should_AllowTotalAmountAccumulation()
    {
        var sale = new Sale();

        sale.TotalAmount += 25.0m;
        sale.TotalAmount += 15.5m;

        sale.TotalAmount.Should().Be(40.5m);
    }

    [Fact]
    public void Sale_Should_AllowItemsToBeAdded()
    {
        var sale = new Sale();

        sale.Items.Add(new SaleItem { Quantity = 2, SubTotal = 20m });
        sale.Items.Add(new SaleItem { Quantity = 1, SubTotal = 10m });

        sale.Items.Should().HaveCount(2);
    }
}
