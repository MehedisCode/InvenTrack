namespace InvenTrack.Application.UnitTests.Features.Sales.Queries.GetAllSales;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Sales.Queries.GetAllSales;
using InvenTrack.Domain.Entities;
using Moq;
using Xunit;

public class GetAllSalesQueryHandlerTests
{
    private readonly Mock<ISaleRepository> _saleRepository = new();

    private GetAllSalesQueryHandler CreateHandler() => new(_saleRepository.Object);

    [Fact]
    public async Task Handle_Should_ReturnAllSales_AsDtos()
    {
        var sales = new List<Sale>
        {
            new()
            {
                Id = Guid.NewGuid(), SaleNumber = "S-1", CustomerName = "Alice",
                Items = new List<SaleItem> { new() { ProductId = Guid.NewGuid(), Quantity = 2, SellingPrice = 5m, SubTotal = 10m } }
            },
            new()
            {
                Id = Guid.NewGuid(), SaleNumber = "S-2", CustomerName = "Bob", Items = new List<SaleItem>()
            }
        };

        _saleRepository
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(sales);

        var handler = CreateHandler();

        var result = await handler.Handle(new GetAllSalesQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
        result[0].SaleNumber.Should().Be("S-1");
        result[0].CustomerName.Should().Be("Alice");
        result[0].Items.Should().HaveCount(1);
        result[0].Items.First().SubTotal.Should().Be(10m);
        result[1].SaleNumber.Should().Be("S-2");
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyList_WhenNoSales()
    {
        _saleRepository
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Sale>());

        var handler = CreateHandler();

        var result = await handler.Handle(new GetAllSalesQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
