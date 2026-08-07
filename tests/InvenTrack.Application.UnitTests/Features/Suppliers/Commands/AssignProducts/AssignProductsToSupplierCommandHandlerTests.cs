namespace InvenTrack.Application.UnitTests.Features.Suppliers.Commands.AssignProducts;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Suppliers.Commands.AssignProducts;
using InvenTrack.Domain.Entities;
using Moq;
using Xunit;

public class AssignProductsToSupplierCommandHandlerTests
{
    private readonly Mock<ISupplierRepository> _supplierRepository = new();
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private AssignProductsToSupplierCommandHandler CreateHandler() => new(
        _supplierRepository.Object, _productRepository.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_Should_AssignProducts_WhenSupplierAndProductsExist()
    {
        var supplierId = Guid.NewGuid();
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();
        var supplier = new Supplier { Id = supplierId, Products = new List<Product>() };

        _supplierRepository
            .Setup(r => r.GetByIdWithProductsAsync(supplierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(supplier);
        _productRepository
            .Setup(r => r.GetByIdAsync(productId1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Product { Id = productId1 });
        _productRepository
            .Setup(r => r.GetByIdAsync(productId2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Product { Id = productId2 });

        var handler = CreateHandler();

        await handler.Handle(new AssignProductsToSupplierCommand(supplierId, new List<Guid> { productId1, productId2 }), CancellationToken.None);

        supplier.Products.Should().HaveCount(2);
        _supplierRepository.Verify(r => r.UpdateAsync(supplier, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_NotDuplicate_WhenProductAlreadyLinked()
    {
        var supplierId = Guid.NewGuid();
        var existingProductId = Guid.NewGuid();
        var existingProduct = new Product { Id = existingProductId };
        var supplier = new Supplier { Id = supplierId, Products = new List<Product> { existingProduct } };

        _supplierRepository
            .Setup(r => r.GetByIdWithProductsAsync(supplierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(supplier);
        _productRepository
            .Setup(r => r.GetByIdAsync(existingProductId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        var handler = CreateHandler();

        await handler.Handle(new AssignProductsToSupplierCommand(supplierId, new List<Guid> { existingProductId }), CancellationToken.None);

        supplier.Products.Should().HaveCount(1);
        supplier.Products.First().Should().Be(existingProduct);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenSupplierDoesNotExist()
    {
        var supplierId = Guid.NewGuid();
        _supplierRepository
            .Setup(r => r.GetByIdWithProductsAsync(supplierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Supplier?)null);

        var handler = CreateHandler();

        await FluentActions.Invoking(() => handler.Handle(
            new AssignProductsToSupplierCommand(supplierId, new List<Guid> { Guid.NewGuid() }), CancellationToken.None))
            .Should().ThrowAsync<Exception>()
            .WithMessage($"*Supplier with ID {supplierId} was not found.*");

        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenProductDoesNotExist()
    {
        var supplierId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var supplier = new Supplier { Id = supplierId, Products = new List<Product>() };

        _supplierRepository
            .Setup(r => r.GetByIdWithProductsAsync(supplierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(supplier);
        _productRepository
            .Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var handler = CreateHandler();

        await FluentActions.Invoking(() => handler.Handle(
            new AssignProductsToSupplierCommand(supplierId, new List<Guid> { productId }), CancellationToken.None))
            .Should().ThrowAsync<Exception>()
            .WithMessage($"*Product with ID {productId} was not found.*");
    }
}
