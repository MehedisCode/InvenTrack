namespace InvenTrack.Application.UnitTests.Features.Purchases.Queries.GetAllPurchases;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Purchases.Queries.GetAllPurchases;
using InvenTrack.Domain.Entities;
using Moq;
using Xunit;

public class GetAllPurchasesQueryHandlerTests
{
    private readonly Mock<IPurchaseRepository> _purchaseRepository = new();

    private GetAllPurchasesQueryHandler CreateHandler() => new(_purchaseRepository.Object);

    [Fact]
    public async Task Handle_Should_ReturnAllPurchases_AsDtos()
    {
        var purchases = new List<Purchase>
        {
            new()
            {
                Id = Guid.NewGuid(), PurchaseNumber = "PO-1",
                Items = new List<PurchaseItem> { new() { ProductId = Guid.NewGuid(), Quantity = 2, UnitCost = 10m, SubTotal = 20m } }
            }
        };

        _purchaseRepository
            .Setup(r => r.GetAllPurchasesAsync(It.IsAny<PurchaseQueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedList<Purchase>(purchases, purchases.Count, 1, 10));

        var handler = CreateHandler();

        var result = await handler.Handle(new GetAllPurchasesQuery(), CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items.First().PurchaseNumber.Should().Be("PO-1");
        result.Items.First().Items.Should().HaveCount(1);
        result.Items.First().Items.First().SubTotal.Should().Be(20m);
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyList_WhenNoPurchases()
    {
        _purchaseRepository
            .Setup(r => r.GetAllPurchasesAsync(It.IsAny<PurchaseQueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedList<Purchase>(new List<Purchase>(), 0, 1, 10));

        var handler = CreateHandler();

        var result = await handler.Handle(new GetAllPurchasesQuery(), CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}
