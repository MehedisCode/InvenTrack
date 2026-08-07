namespace InvenTrack.Application.UnitTests.Features.Suppliers.Queries.GetSupplierById;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Suppliers.Queries.GetSupplierById;
using InvenTrack.Domain.Entities;
using Moq;
using Xunit;

public class GetSupplierByIdQueryHandlerTests
{
    private readonly Mock<ISupplierRepository> _supplierRepository = new();

    private GetSupplierByIdQueryHandler CreateHandler() => new(_supplierRepository.Object);

    [Fact]
    public async Task Handle_Should_ReturnSupplierDto_WhenSupplierExists()
    {
        var supplierId = Guid.NewGuid();
        var supplier = new Supplier
        {
            Id = supplierId, CompanyName = "Acme", ContactPerson = "John", Email = "a@b.com", Phone = "555",
            Products = new List<Product> { new() { Name = "Widget" }, new() { Name = "Gadget" } }
        };

        _supplierRepository
            .Setup(r => r.GetByIdWithProductsAsync(supplierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(supplier);

        var handler = CreateHandler();

        var result = await handler.Handle(new GetSupplierByIdQuery(supplierId), CancellationToken.None);

        result.Should().NotBeNull();
        result!.CompanyName.Should().Be("Acme");
        result.Products.Should().HaveCount(2);
        result.Products.Should().ContainInOrder("Widget", "Gadget");
    }

    [Fact]
    public async Task Handle_Should_ReturnNull_WhenSupplierDoesNotExist()
    {
        var supplierId = Guid.NewGuid();
        _supplierRepository
            .Setup(r => r.GetByIdWithProductsAsync(supplierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Supplier?)null);

        var handler = CreateHandler();

        var result = await handler.Handle(new GetSupplierByIdQuery(supplierId), CancellationToken.None);

        result.Should().BeNull();
    }
}
