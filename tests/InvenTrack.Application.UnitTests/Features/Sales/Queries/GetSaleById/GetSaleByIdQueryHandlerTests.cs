namespace InvenTrack.Application.UnitTests.Features.Sales.Queries.GetSaleById;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Sales.Queries.GetSaleById;
using InvenTrack.Domain.Entities;
using Moq;
using Xunit;

public class GetSaleByIdQueryHandlerTests
{
    private readonly Mock<ISaleRepository> _saleRepository = new();

    private GetSaleByIdQueryHandler CreateHandler() => new(_saleRepository.Object);

    [Fact]
    public async Task Handle_Should_ReturnSaleDto_WhenSaleExists()
    {
        var saleId = Guid.NewGuid();
        var sale = new Sale
        {
            Id = saleId, SaleNumber = "S-1", CustomerName = "Alice", UserId = Guid.NewGuid(),
            TotalAmount = 100m,
            Items = new List<SaleItem> { new() { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 2, SellingPrice = 50m, SubTotal = 100m } }
        };

        _saleRepository
            .Setup(r => r.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        var handler = CreateHandler();

        var result = await handler.Handle(new GetSaleByIdQuery(saleId), CancellationToken.None);

        result.Should().NotBeNull();
        result!.SaleNumber.Should().Be("S-1");
        result.CustomerName.Should().Be("Alice");
        result.TotalAmount.Should().Be(100m);
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_Should_ReturnNull_WhenSaleDoesNotExist()
    {
        var saleId = Guid.NewGuid();
        _saleRepository
            .Setup(r => r.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sale?)null);

        var handler = CreateHandler();

        var result = await handler.Handle(new GetSaleByIdQuery(saleId), CancellationToken.None);

        result.Should().BeNull();
    }
}
