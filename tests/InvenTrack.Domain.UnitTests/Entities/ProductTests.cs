namespace InvenTrack.Domain.UnitTests.Entities;

using FluentAssertions;
using InvenTrack.Domain.Entities;
using Xunit;

public class ProductTests
{
    [Fact]
    public void Product_Creation_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var name = "Test Product";
        var description = "Test Description";
        var sku = "SKU-123";

        // Act
        var product = new Product
        {
            Id = productId,
            CategoryId = categoryId,
            Name = name,
            Description = description,
            SKU = sku,
            QuantityInStock = 10,
            PurchasePrice = 50.0m,
            SellingPrice = 100.0m,
            IsActive = true
        };

        // Assert
        product.Id.Should().Be(productId);
        product.Name.Should().Be(name);
        product.SKU.Should().Be(sku);
        product.QuantityInStock.Should().Be(10);
        product.IsActive.Should().BeTrue();
    }
}
