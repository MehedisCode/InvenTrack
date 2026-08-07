namespace InvenTrack.Application.UnitTests.Features.Products.Queries.GetProductById;

using AutoMapper;
using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Products.DTOs;
using InvenTrack.Application.Features.Products.Queries.GetProductById;
using InvenTrack.Domain.Entities;
using Moq;
using Xunit;

public class GetProductByIdQueryHandlerTests
{
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly Mock<IMapper> _mapper = new();

    private GetProductByIdQueryHandler CreateHandler() => new(_productRepository.Object, _mapper.Object);

    [Fact]
    public async Task Handle_Should_ReturnMappedProduct_WhenProductExists()
    {
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, Name = "Laptop", SKU = "LAP" };
        var dto = new ProductDto { Id = productId, Name = "Laptop", SKU = "LAP" };

        _productRepository
            .Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _mapper
            .Setup(m => m.Map<ProductDto>(product))
            .Returns(dto);

        var handler = CreateHandler();

        var result = await handler.Handle(new GetProductByIdQuery(productId), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(productId);
        result.Name.Should().Be("Laptop");
        _mapper.Verify(m => m.Map<ProductDto>(product), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowKeyNotFoundException_WhenProductDoesNotExist()
    {
        var productId = Guid.NewGuid();
        _productRepository
            .Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var handler = CreateHandler();

        await FluentActions.Invoking(() => handler.Handle(new GetProductByIdQuery(productId), CancellationToken.None))
            .Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*'{productId}'*");
    }
}
