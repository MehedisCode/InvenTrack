namespace InvenTrack.Application.UnitTests.Features.Purchases.Commands.CreatePurchase;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Purchases.Commands.CreatePurchase;
using InvenTrack.Domain.Entities;
using InvenTrack.Domain.Enums;
using Moq;
using Xunit;

public class CreatePurchaseCommandHandlerTests
{
    private readonly Mock<IPurchaseRepository> _purchaseRepository = new();
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly Mock<IInventoryRepository> _inventoryRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();

    private CreatePurchaseCommandHandler CreateHandler() => new(
        _purchaseRepository.Object, _productRepository.Object, _inventoryRepository.Object,
        _unitOfWork.Object, _currentUserService.Object);

    [Fact]
    public async Task Handle_Should_CreatePurchase_IncreaseStock_AndReturnDto()
    {
        var productId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var product = new Product { Id = productId, QuantityInStock = 10, PurchasePrice = 50m };

        _productRepository.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        _purchaseRepository
            .Setup(r => r.AddAsync(It.IsAny<Purchase>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Purchase p, CancellationToken _) => p);
        _inventoryRepository
            .Setup(r => r.AddTransactionAsync(It.IsAny<StockTransaction>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((StockTransaction t, CancellationToken _) => t);
        _currentUserService.SetupGet(s => s.UserId).Returns(userId);

        var handler = CreateHandler();
        var command = new CreatePurchaseCommand("PO-1", Guid.NewGuid(), new List<CreatePurchaseCommandItem>
        {
            new() { ProductId = productId, Quantity = 3 }
        });

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.PurchaseNumber.Should().Be("PO-1");
        result.TotalCost.Should().Be(150m);
        result.Items.Should().HaveCount(1);
        result.Items.First().SubTotal.Should().Be(150m);
        product.QuantityInStock.Should().Be(13);
        _inventoryRepository.Verify(r => r.AddTransactionAsync(It.Is<StockTransaction>(t =>
            t.ProductId == productId && t.Quantity == 3 && t.TransactionType == TransactionType.StockIn &&
            t.Remarks == "Purchase PO-1" && t.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenProductDoesNotExist()
    {
        var productId = Guid.NewGuid();
        _productRepository.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);

        var handler = CreateHandler();
        var command = new CreatePurchaseCommand("PO-1", Guid.NewGuid(), new List<CreatePurchaseCommandItem>
        {
            new() { ProductId = productId, Quantity = 1 }
        });

        await FluentActions.Invoking(() => handler.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<Exception>()
            .WithMessage($"*Product with Id {productId} not found*");

        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
