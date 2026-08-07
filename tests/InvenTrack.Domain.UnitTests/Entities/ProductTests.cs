namespace InvenTrack.Domain.UnitTests.Entities;

using FluentAssertions;
using InvenTrack.Domain.Entities;
using Xunit;

public class ProductTests
{
    [Fact]
    public void Product_Should_HaveDefaultValues()
    {
        var product = new Product();

        product.Id.Should().NotBe(Guid.Empty);
        product.IsActive.Should().BeTrue();
        product.QuantityInStock.Should().Be(0);
        product.StockTransactions.Should().NotBeNull();
        product.Suppliers.Should().NotBeNull();
        product.StockTransactions.Should().BeEmpty();
        product.Suppliers.Should().BeEmpty();
    }

    [Fact]
    public void Product_Creation_ShouldSetPropertiesCorrectly()
    {
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        var product = new Product
        {
            Id = productId,
            CategoryId = categoryId,
            Name = "Test Product",
            Description = "Test Description",
            SKU = "SKU-123",
            QuantityInStock = 10,
            PurchasePrice = 50.0m,
            SellingPrice = 100.0m,
            IsActive = true
        };

        product.Id.Should().Be(productId);
        product.Name.Should().Be("Test Product");
        product.SKU.Should().Be("SKU-123");
        product.QuantityInStock.Should().Be(10);
        product.PurchasePrice.Should().Be(50.0m);
        product.SellingPrice.Should().Be(100.0m);
        product.CategoryId.Should().Be(categoryId);
        product.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Product_Should_AllowStockManipulation()
    {
        var product = new Product { QuantityInStock = 10 };

        product.QuantityInStock += 5;
        product.QuantityInStock -= 3;

        product.QuantityInStock.Should().Be(12);
    }
}
