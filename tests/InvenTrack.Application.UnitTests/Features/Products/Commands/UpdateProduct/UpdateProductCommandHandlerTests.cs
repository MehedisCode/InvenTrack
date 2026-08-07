namespace InvenTrack.Application.UnitTests.Features.Products.Commands.UpdateProduct;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Products.Commands.UpdateProduct;
using InvenTrack.Domain.Entities;
using Moq;
using Xunit;

public class UpdateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private UpdateProductCommandHandler CreateHandler() => new(_productRepository.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_Should_UpdateProduct_WhenProductExists()
    {
        var productId = Guid.NewGuid();
        var existing = new Product
        {
            Id = productId, Name = "Old", SKU = "OLD", Description = "Old",
            PurchasePrice = 10m, SellingPrice = 20m, CategoryId = Guid.NewGuid(), IsActive = true
        };

        _productRepository
            .Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var handler = CreateHandler();
        var newCategoryId = Guid.NewGuid();

        var result = await handler.Handle(new UpdateProductCommand(
            productId, "New", "NEW", "New", 15m, 25m, newCategoryId, false), CancellationToken.None);

        result.Name.Should().Be("New");
        result.SKU.Should().Be("NEW");
        result.PurchasePrice.Should().Be(15m);
        result.SellingPrice.Should().Be(25m);
        result.CategoryId.Should().Be(newCategoryId);
        _productRepository.Verify(r => r.UpdateAsync(It.Is<Product>(p =>
            p.Id == productId && p.Name == "New" && p.SKU == "NEW"), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowKeyNotFoundException_WhenProductDoesNotExist()
    {
        var productId = Guid.NewGuid();
        _productRepository
            .Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var handler = CreateHandler();

        await FluentActions.Invoking(() => handler.Handle(
            new UpdateProductCommand(productId, "New", "NEW", "d", 1m, 2m, Guid.NewGuid(), true), CancellationToken.None))
            .Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*'{productId}'*");

        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
