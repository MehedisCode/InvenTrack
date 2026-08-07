namespace InvenTrack.Application.UnitTests.Features.Suppliers.Queries.SearchSuppliers;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Suppliers.Queries.SearchSuppliers;
using InvenTrack.Domain.Entities;
using Moq;
using Xunit;

public class SearchSuppliersQueryHandlerTests
{
    private readonly Mock<ISupplierRepository> _supplierRepository = new();

    private SearchSuppliersQueryHandler CreateHandler() => new(_supplierRepository.Object);

    [Fact]
    public async Task Handle_Should_ReturnSearchResults_AsDtos()
    {
        var suppliers = new List<Supplier>
        {
            new() { Id = Guid.NewGuid(), CompanyName = "Acme", ContactPerson = "John", Email = "a@b.com", Phone = "555" }
        };

        _supplierRepository
            .Setup(r => r.SearchAsync("acme", It.IsAny<CancellationToken>()))
            .ReturnsAsync(suppliers);

        var handler = CreateHandler();

        var result = await handler.Handle(new SearchSuppliersQuery("acme"), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].CompanyName.Should().Be("Acme");
        _supplierRepository.Verify(r => r.SearchAsync("acme", It.IsAny<CancellationToken>()), Times.Once);
    }
}
