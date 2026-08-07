namespace InvenTrack.Application.UnitTests.Features.Purchases.Queries.GetAllPurchases;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
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
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(purchases);

        var handler = CreateHandler();

        var result = await handler.Handle(new GetAllPurchasesQuery(), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].PurchaseNumber.Should().Be("PO-1");
        result[0].Items.Should().HaveCount(1);
        result[0].Items.First().SubTotal.Should().Be(20m);
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyList_WhenNoPurchases()
    {
        _purchaseRepository
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Purchase>());

        var handler = CreateHandler();

        var result = await handler.Handle(new GetAllPurchasesQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
