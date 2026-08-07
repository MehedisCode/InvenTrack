namespace InvenTrack.Application.UnitTests.Features.Sales.Queries.GetAllSales;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
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
            .Setup(r => r.GetAllSalesAsync(It.IsAny<SaleQueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedList<Sale>(sales, sales.Count, 1, 10));

        var handler = CreateHandler();

        var result = await handler.Handle(new GetAllSalesQuery(), CancellationToken.None);

        result.Items.Should().HaveCount(2);
        result.Items.First().SaleNumber.Should().Be("S-1");
        result.Items.First().CustomerName.Should().Be("Alice");
        result.Items.First().Items.Should().HaveCount(1);
        result.Items.First().Items.First().SubTotal.Should().Be(10m);
        result.Items.ElementAt(1).SaleNumber.Should().Be("S-2");
        result.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyList_WhenNoSales()
    {
        _saleRepository
            .Setup(r => r.GetAllSalesAsync(It.IsAny<SaleQueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedList<Sale>(new List<Sale>(), 0, 1, 10));

        var handler = CreateHandler();

        var result = await handler.Handle(new GetAllSalesQuery(), CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}
