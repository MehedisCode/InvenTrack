namespace InvenTrack.Application.UnitTests.Features.Sales.Commands.RefundSale;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Sales.Commands.RefundSale;
using InvenTrack.Domain.Entities;
using InvenTrack.Domain.Enums;
using Moq;
using Xunit;

public class RefundSaleCommandHandlerTests
{
    private readonly Mock<ISaleRepository> _saleRepository = new();
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly Mock<IInventoryRepository> _inventoryRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();

    private RefundSaleCommandHandler CreateHandler() => new(
        _saleRepository.Object, _productRepository.Object, _inventoryRepository.Object,
        _unitOfWork.Object, _currentUserService.Object);

    [Fact]
    public async Task Handle_Should_ReturnTrue_AndRestoreStock_WhenSaleExists()
    {
        var saleId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var sale = new Sale
        {
            Id = saleId,
            SaleNumber = "S-1",
            Items = new List<SaleItem> { new() { ProductId = productId, Quantity = 3 } }
        };
        var product = new Product { Id = productId, QuantityInStock = 5 };

        _saleRepository.Setup(r => r.GetByIdAsync(saleId, It.IsAny<CancellationToken>())).ReturnsAsync(sale);
        _productRepository.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        _currentUserService.SetupGet(s => s.UserId).Returns(userId);

        var handler = CreateHandler();

        var result = await handler.Handle(new RefundSaleCommand(saleId), CancellationToken.None);

        result.Should().BeTrue();
        product.QuantityInStock.Should().Be(8);
        _productRepository.Verify(r => r.UpdateAsync(It.Is<Product>(p => p.Id == productId && p.QuantityInStock == 8), It.IsAny<CancellationToken>()), Times.Once);
        _inventoryRepository.Verify(r => r.AddTransactionAsync(It.Is<StockTransaction>(t =>
            t.ProductId == productId && t.Quantity == 3 && t.TransactionType == TransactionType.StockIn &&
            t.Remarks == "Refund for Sale S-1" && t.UserId == userId && t.SaleId == saleId), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenSaleDoesNotExist()
    {
        var saleId = Guid.NewGuid();
        _saleRepository.Setup(r => r.GetByIdAsync(saleId, It.IsAny<CancellationToken>())).ReturnsAsync((Sale?)null);

        var handler = CreateHandler();

        await FluentActions.Invoking(() => handler.Handle(new RefundSaleCommand(saleId), CancellationToken.None))
            .Should().ThrowAsync<Exception>()
            .WithMessage($"*Sale with Id {saleId} not found*");

        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_SkipProductUpdate_WhenProductNotFound()
    {
        var saleId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var sale = new Sale
        {
            Id = saleId, SaleNumber = "S-2",
            Items = new List<SaleItem> { new() { ProductId = productId, Quantity = 2 } }
        };

        _saleRepository.Setup(r => r.GetByIdAsync(saleId, It.IsAny<CancellationToken>())).ReturnsAsync(sale);
        _productRepository.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);

        var handler = CreateHandler();

        var result = await handler.Handle(new RefundSaleCommand(saleId), CancellationToken.None);

        result.Should().BeTrue();
        _productRepository.Verify(r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
        _inventoryRepository.Verify(r => r.AddTransactionAsync(It.IsAny<StockTransaction>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
