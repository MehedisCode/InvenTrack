namespace InvenTrack.Application.UnitTests.Features.Products.Commands.CreateProduct;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Products.Commands.CreateProduct;
using InvenTrack.Domain.Entities;
using Moq;
using Xunit;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private CreateProductCommandHandler CreateHandler() => new(_productRepository.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_Should_CreateProduct_WithZeroInitialStock()
    {
        _productRepository
            .Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product p, CancellationToken _) => p);

        var handler = CreateHandler();
        var command = new CreateProductCommand("Laptop", "LAP-001", "Gaming laptop", 800m, 1200m, Guid.NewGuid());

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be("Laptop");
        result.SKU.Should().Be("LAP-001");
        result.QuantityInStock.Should().Be(0);
        result.PurchasePrice.Should().Be(800m);
        result.SellingPrice.Should().Be(1200m);
        _productRepository.Verify(r => r.AddAsync(It.Is<Product>(p =>
            p.Name == "Laptop" && p.QuantityInStock == 0 && p.IsActive), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
