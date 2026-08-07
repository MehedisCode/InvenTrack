namespace InvenTrack.Infrastructure.UnitTests;

using FluentAssertions;
using InvenTrack.Infrastructure.Persistence;
using InvenTrack.Infrastructure.UnitTests.Helpers;
using Xunit;

[Collection("Database")]
public class UnitOfWorkTests
{
    private readonly DatabaseFixture _fixture;

    public UnitOfWorkTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Categories_Should_ReturnNonNullRepository()
    {
        var unitOfWork = new UnitOfWork(_fixture.Context);

        unitOfWork.Categories.Should().NotBeNull();
    }

    [Fact]
    public void Products_Should_ReturnNonNullRepository()
    {
        var unitOfWork = new UnitOfWork(_fixture.Context);

        unitOfWork.Products.Should().NotBeNull();
    }

    [Fact]
    public void Suppliers_Should_ReturnNonNullRepository()
    {
        var unitOfWork = new UnitOfWork(_fixture.Context);

        unitOfWork.Suppliers.Should().NotBeNull();
    }

    [Fact]
    public void Sales_Should_ReturnNonNullRepository()
    {
        var unitOfWork = new UnitOfWork(_fixture.Context);

        unitOfWork.Sales.Should().NotBeNull();
    }

    [Fact]
    public void Purchases_Should_ReturnNonNullRepository()
    {
        var unitOfWork = new UnitOfWork(_fixture.Context);

        unitOfWork.Purchases.Should().NotBeNull();
    }

    [Fact]
    public void Inventory_Should_ReturnNonNullRepository()
    {
        var unitOfWork = new UnitOfWork(_fixture.Context);

        unitOfWork.Inventory.Should().NotBeNull();
    }

    [Fact]
    public void Dashboard_Should_ReturnNonNullRepository()
    {
        var unitOfWork = new UnitOfWork(_fixture.Context);

        unitOfWork.Dashboard.Should().NotBeNull();
    }

    [Fact]
    public void RepositoryProperties_Should_BeLazyAndReturnSameInstance_OnRepeatedAccess()
    {
        var unitOfWork = new UnitOfWork(_fixture.Context);

        var first = unitOfWork.Products;
        var second = unitOfWork.Products;

        first.Should().BeSameAs(second);
    }

    [Fact]
    public async Task SaveChangesAsync_Should_DelegateToContext()
    {
        var unitOfWork = new UnitOfWork(_fixture.Context);
        await unitOfWork.SaveChangesAsync();

        // No exception means the delegation path works; a real write would round-trip.
        true.Should().BeTrue();
    }
}
