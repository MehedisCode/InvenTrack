namespace InvenTrack.Application.UnitTests.Features.Products.Commands.DeleteProduct;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Products.Commands.DeleteProduct;
using InvenTrack.Domain.Entities;
using Moq;
using Xunit;

public class DeleteProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private DeleteProductCommandHandler CreateHandler() => new(_productRepository.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_Should_SoftDeleteProduct_WhenProductExists()
    {
        var productId = Guid.NewGuid();
        var existing = new Product { Id = productId, Name = "Laptop", IsActive = true };

        _productRepository
            .Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var handler = CreateHandler();

        await handler.Handle(new DeleteProductCommand(productId), CancellationToken.None);

        _productRepository.Verify(r => r.UpdateAsync(It.Is<Product>(p =>
            p.Id == productId && p.IsActive == false), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenProductDoesNotExist()
    {
        var productId = Guid.NewGuid();
        _productRepository
            .Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var handler = CreateHandler();

        await FluentActions.Invoking(() => handler.Handle(new DeleteProductCommand(productId), CancellationToken.None))
            .Should().ThrowAsync<Exception>()
            .WithMessage("*Product not found.*");

        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
