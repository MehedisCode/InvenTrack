namespace InvenTrack.Domain.UnitTests.Entities;

using FluentAssertions;
using InvenTrack.Domain.Entities;
using Xunit;

public class PurchaseTests
{
    [Fact]
    public void Purchase_Should_HaveDefaultValues()
    {
        var purchase = new Purchase();

        purchase.Id.Should().NotBe(Guid.Empty);
        purchase.Items.Should().NotBeNull().And.BeEmpty();
        purchase.StockTransactions.Should().NotBeNull().And.BeEmpty();
        purchase.TotalCost.Should().Be(0);
    }

    [Fact]
    public void Purchase_Should_AllowTotalCostAccumulation()
    {
        var purchase = new Purchase();

        purchase.TotalCost += 100m;
        purchase.TotalCost += 50m;

        purchase.TotalCost.Should().Be(150m);
    }
}
