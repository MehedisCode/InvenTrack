namespace InvenTrack.Application.UnitTests.Features.Inventory.Commands.StockOut;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Inventory.Commands.StockOut;
using InvenTrack.Domain.Entities;
using InvenTrack.Domain.Enums;
using Moq;
using Xunit;

public class StockOutCommandHandlerTests
{
    private readonly Mock<IInventoryRepository> _inventoryRepository = new();
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private StockOutCommandHandler CreateHandler() => new(
        _inventoryRepository.Object, _productRepository.Object, _currentUserService.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_Should_DecreaseStock_WhenSufficientStock()
    {
        var productId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var product = new Product { Id = productId, QuantityInStock = 10 };

        _productRepository.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        _inventoryRepository
            .Setup(r => r.AddTransactionAsync(It.IsAny<StockTransaction>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((StockTransaction t, CancellationToken _) => t);
        _currentUserService.SetupGet(s => s.UserId).Returns(userId);

        var handler = CreateHandler();

        var result = await handler.Handle(new StockOutCommand(productId, 4, "Manual adjustment"), CancellationToken.None);

        result.Should().NotBe(Guid.Empty);
        product.QuantityInStock.Should().Be(6);
        _inventoryRepository.Verify(r => r.AddTransactionAsync(It.Is<StockTransaction>(t =>
            t.TransactionType == TransactionType.StockOut && t.Quantity == 4), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenProductDoesNotExist()
    {
        var productId = Guid.NewGuid();
        _productRepository.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);

        var handler = CreateHandler();

        await FluentActions.Invoking(() => handler.Handle(new StockOutCommand(productId, 1, "x"), CancellationToken.None))
            .Should().ThrowAsync<Exception>()
            .WithMessage("*Product not found*");

        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenInsufficientStock()
    {
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, QuantityInStock = 3 };

        _productRepository.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync(product);

        var handler = CreateHandler();

        await FluentActions.Invoking(() => handler.Handle(new StockOutCommand(productId, 5, "x"), CancellationToken.None))
            .Should().ThrowAsync<Exception>()
            .WithMessage("*Insufficient stock*");

        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
