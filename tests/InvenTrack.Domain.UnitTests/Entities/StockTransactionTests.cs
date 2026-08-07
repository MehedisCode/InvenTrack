namespace InvenTrack.Domain.UnitTests.Entities;

using FluentAssertions;
using InvenTrack.Domain.Entities;
using InvenTrack.Domain.Enums;
using Xunit;

public class StockTransactionTests
{
    [Fact]
    public void StockTransaction_Should_HaveDefaultValues()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var transaction = new StockTransaction();
        var after = DateTime.UtcNow.AddSeconds(1);

        transaction.Id.Should().NotBe(Guid.Empty);
        transaction.TransactionDate.Should().BeOnOrAfter(before);
        transaction.TransactionDate.Should().BeOnOrBefore(after);
        transaction.Quantity.Should().Be(0);
    }

    [Fact]
    public void StockTransaction_Should_AllowOptionalSaleAndPurchaseReferences()
    {
        var transaction = new StockTransaction
        {
            SaleId = null,
            PurchaseId = null
        };

        transaction.SaleId.Should().BeNull();
        transaction.PurchaseId.Should().BeNull();

        var saleId = Guid.NewGuid();
        var purchaseId = Guid.NewGuid();
        transaction.SaleId = saleId;
        transaction.PurchaseId = purchaseId;

        transaction.SaleId.Should().Be(saleId);
        transaction.PurchaseId.Should().Be(purchaseId);
    }

    [Theory]
    [InlineData(TransactionType.StockIn)]
    [InlineData(TransactionType.StockOut)]
    public void StockTransaction_Should_HoldTransactionType(TransactionType type)
    {
        var transaction = new StockTransaction { TransactionType = type };

        transaction.TransactionType.Should().Be(type);
    }
}
