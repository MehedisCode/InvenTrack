namespace InvenTrack.Application.UnitTests.Features.Products.Queries.GetProducts;

using AutoMapper;
using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Products.DTOs;
using InvenTrack.Application.Features.Products.Queries.GetProducts;
using InvenTrack.Domain.Entities;
using Moq;
using Xunit;

public class GetProductsQueryHandlerTests
{
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly Mock<IMapper> _mapper = new();

    private GetProductsQueryHandler CreateHandler() => new(_productRepository.Object, _mapper.Object);

    [Fact]
    public async Task Handle_Should_ReturnPaginatedMappedProducts()
    {
        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), Name = "A" },
            new() { Id = Guid.NewGuid(), Name = "B" }
        };
        var dtos = new List<ProductDto>
        {
            new() { Name = "A" },
            new() { Name = "B" }
        };
        var paginated = new PaginatedList<Product>(products, 10, 1, 2);

        _productRepository
            .Setup(r => r.GetProductsAsync(It.IsAny<ProductQueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginated);
        _mapper
            .Setup(m => m.Map<List<ProductDto>>(It.IsAny<List<Product>>()))
            .Returns(dtos);

        var handler = CreateHandler();

        var result = await handler.Handle(new GetProductsQuery(), CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(10);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(2);
        _mapper.Verify(m => m.Map<List<ProductDto>>(products), Times.Once);
    }
}
