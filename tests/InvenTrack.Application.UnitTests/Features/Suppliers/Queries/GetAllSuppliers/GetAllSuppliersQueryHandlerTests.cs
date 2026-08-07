namespace InvenTrack.Application.UnitTests.Features.Suppliers.Queries.GetAllSuppliers;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Suppliers.Queries.GetAllSuppliers;
using InvenTrack.Domain.Entities;
using Moq;
using Xunit;

public class GetAllSuppliersQueryHandlerTests
{
    private readonly Mock<ISupplierRepository> _supplierRepository = new();

    private GetAllSuppliersQueryHandler CreateHandler() => new(_supplierRepository.Object);

    [Fact]
    public async Task Handle_Should_ReturnAllSuppliers_AsDtos()
    {
        var suppliers = new List<Supplier>
        {
            new() { Id = Guid.NewGuid(), CompanyName = "Acme", Products = new List<Product> { new() { Name = "Widget" } } },
            new() { Id = Guid.NewGuid(), CompanyName = "Beta", Products = new List<Product>() }
        };

        _supplierRepository
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(suppliers);

        var handler = CreateHandler();

        var result = await handler.Handle(new GetAllSuppliersQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
        result[0].CompanyName.Should().Be("Acme");
        result[0].Products.Should().ContainSingle().Which.Should().Be("Widget");
        result[1].CompanyName.Should().Be("Beta");
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyList_WhenNoSuppliers()
    {
        _supplierRepository
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Supplier>());

        var handler = CreateHandler();

        var result = await handler.Handle(new GetAllSuppliersQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
