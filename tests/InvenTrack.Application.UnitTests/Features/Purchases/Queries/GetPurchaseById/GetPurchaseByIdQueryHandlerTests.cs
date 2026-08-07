namespace InvenTrack.Application.UnitTests.Features.Purchases.Queries.GetPurchaseById;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Purchases.Queries.GetPurchaseById;
using InvenTrack.Domain.Entities;
using Moq;
using Xunit;

public class GetPurchaseByIdQueryHandlerTests
{
    private readonly Mock<IPurchaseRepository> _purchaseRepository = new();

    private GetPurchaseByIdQueryHandler CreateHandler() => new(_purchaseRepository.Object);

    [Fact]
    public async Task Handle_Should_ReturnPurchaseDto_WhenPurchaseExists()
    {
        var purchaseId = Guid.NewGuid();
        var purchase = new Purchase
        {
            Id = purchaseId, PurchaseNumber = "PO-1", TotalCost = 100m,
            Items = new List<PurchaseItem> { new() { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 1, UnitCost = 100m, SubTotal = 100m } }
        };

        _purchaseRepository
            .Setup(r => r.GetByIdAsync(purchaseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(purchase);

        var handler = CreateHandler();

        var result = await handler.Handle(new GetPurchaseByIdQuery(purchaseId), CancellationToken.None);

        result.Should().NotBeNull();
        result!.PurchaseNumber.Should().Be("PO-1");
        result.TotalCost.Should().Be(100m);
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_Should_ReturnNull_WhenPurchaseDoesNotExist()
    {
        var purchaseId = Guid.NewGuid();
        _purchaseRepository
            .Setup(r => r.GetByIdAsync(purchaseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Purchase?)null);

        var handler = CreateHandler();

        var result = await handler.Handle(new GetPurchaseByIdQuery(purchaseId), CancellationToken.None);

        result.Should().BeNull();
    }
}
