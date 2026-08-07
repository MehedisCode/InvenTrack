namespace InvenTrack.Domain.UnitTests.Entities;

using FluentAssertions;
using InvenTrack.Domain.Entities;
using Xunit;

public class CategoryTests
{
    [Fact]
    public void Category_Should_HaveDefaultValues()
    {
        var category = new Category();

        category.Id.Should().NotBe(Guid.Empty);
        category.Name.Should().Be(string.Empty);
        category.Description.Should().BeNull();
        category.Products.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Category_Should_AllowProductsToBeAdded()
    {
        var category = new Category();

        category.Products.Add(new Product { Name = "Item" });

        category.Products.Should().HaveCount(1);
    }
}
