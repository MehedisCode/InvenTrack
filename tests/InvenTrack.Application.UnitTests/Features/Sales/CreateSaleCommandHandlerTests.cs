namespace InvenTrack.Application.UnitTests.Features.Sales;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Sales.Commands.CreateSale;
using InvenTrack.Domain.Entities;
using InvenTrack.Domain.Enums;
using Moq;
using Xunit;

public class CreateSaleCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ISaleRepository> _saleRepository = new();
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly Mock<IInventoryRepository> _inventoryRepository = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();

    private CreateSaleCommandHandler CreateHandler() => new(
        _saleRepository.Object,
        _productRepository.Object,
        _inventoryRepository.Object,
        _unitOfWork.Object,
        _currentUserService.Object);

    [Fact]
    public async Task Handle_Should_CreateSale_When_RequestIsValid()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var product = NewProduct(productId, quantityInStock: 10, sellingPrice: 5m);

        _productRepository
            .Setup(repository => repository.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _saleRepository
            .Setup(repository => repository.AddAsync(It.IsAny<Sale>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sale sale, CancellationToken _) => sale);

        _inventoryRepository
            .Setup(repository => repository.AddTransactionAsync(It.IsAny<StockTransaction>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((StockTransaction transaction, CancellationToken _) => transaction);

        _currentUserService.SetupGet(service => service.UserId).Returns(userId);

        var handler = CreateHandler();
        var request = CreateSaleCommand(productId, quantity: 2);

        var result = await handler.Handle(request, CancellationToken.None);

        _unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _saleRepository.Verify(repository => repository.AddAsync(It.IsAny<Sale>(), It.IsAny<CancellationToken>()), Times.Once);
        _productRepository.Verify(repository => repository.UpdateAsync(It.Is<Product>(p => p.Id == productId && p.QuantityInStock == 8), It.IsAny<CancellationToken>()), Times.Once);
        _inventoryRepository.Verify(repository => repository.AddTransactionAsync(
            It.Is<StockTransaction>(transaction =>
                transaction.ProductId == productId &&
                transaction.Quantity == 2 &&
                transaction.TransactionType == TransactionType.StockOut &&
                transaction.Remarks == "Sale S-1001"),
            It.IsAny<CancellationToken>()),
            Times.Once);

        result.TotalAmount.Should().Be(10m);
        result.Items.Should().ContainSingle().Which.SubTotal.Should().Be(10m);
    }

    [Fact]
    public async Task Handle_Should_ThrowUnauthorizedAccessException_When_UserIdIsNull()
    {
        _currentUserService.SetupGet(service => service.UserId).Returns((Guid?)null);

        var handler = CreateHandler();
        var request = CreateSaleCommand(Guid.NewGuid(), quantity: 1);

        Func<Task> act = async () => await handler.Handle(request, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Current user could not be determined.");

        _unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ThrowInvalidOperationException_When_ProductDoesNotExist()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        _currentUserService.SetupGet(service => service.UserId).Returns(userId);
        _productRepository
            .Setup(repository => repository.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var handler = CreateHandler();
        var request = CreateSaleCommand(productId, quantity: 1);

        Func<Task> act = async () => await handler.Handle(request, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Product '{productId}' was not found.");

        _unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ThrowInvalidOperationException_When_StockIsInsufficient()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var product = NewProduct(productId, quantityInStock: 1, sellingPrice: 5m);

        _currentUserService.SetupGet(service => service.UserId).Returns(userId);
        _productRepository
            .Setup(repository => repository.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var handler = CreateHandler();
        var request = CreateSaleCommand(productId, quantity: 2);

        Func<Task> act = async () => await handler.Handle(request, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Insufficient stock for product 'Test product'.");

        _unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_RollbackTransaction_When_SaleRepositoryThrows()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var product = NewProduct(productId, quantityInStock: 10, sellingPrice: 5m);

        _currentUserService.SetupGet(service => service.UserId).Returns(userId);
        _productRepository
            .Setup(repository => repository.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _saleRepository
            .Setup(repository => repository.AddAsync(It.IsAny<Sale>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Unable to persist sale."));
        _inventoryRepository
            .Setup(repository => repository.AddTransactionAsync(It.IsAny<StockTransaction>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((StockTransaction transaction, CancellationToken _) => transaction);

        var handler = CreateHandler();
        var request = CreateSaleCommand(productId, quantity: 2);

        Func<Task> act = async () => await handler.Handle(request, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Unable to persist sale.");

        _unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private static CreateSaleCommand CreateSaleCommand(Guid productId, int quantity)
        => new(
            "S-1001",
            "Alice",
            new List<CreateSaleCommandItem>
            {
                new() { ProductId = productId, Quantity = quantity }
            });

    private static Product NewProduct(Guid productId, int quantityInStock, decimal sellingPrice)
        => new()
        {
            Id = productId,
            Name = "Test product",
            QuantityInStock = quantityInStock,
            SellingPrice = sellingPrice
        };
}
