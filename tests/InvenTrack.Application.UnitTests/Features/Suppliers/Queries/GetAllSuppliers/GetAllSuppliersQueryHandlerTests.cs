namespace InvenTrack.Application.UnitTests.Features.Suppliers.Queries.GetAllSuppliers;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
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
            .Setup(r => r.GetAllSuppliersAsync(It.IsAny<SupplierQueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedList<Supplier>(suppliers, suppliers.Count, 1, 10));

        var handler = CreateHandler();

        var result = await handler.Handle(new GetAllSuppliersQuery(), CancellationToken.None);

        result.Items.Should().HaveCount(2);
        result.Items.First().CompanyName.Should().Be("Acme");
        result.Items.First().Products.Should().ContainSingle().Which.Should().Be("Widget");
        result.Items.ElementAt(1).CompanyName.Should().Be("Beta");
        result.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyList_WhenNoSuppliers()
    {
        _supplierRepository
            .Setup(r => r.GetAllSuppliersAsync(It.IsAny<SupplierQueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedList<Supplier>(new List<Supplier>(), 0, 1, 10));

        var handler = CreateHandler();

        var result = await handler.Handle(new GetAllSuppliersQuery(), CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}
