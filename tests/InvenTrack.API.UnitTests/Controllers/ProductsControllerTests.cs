namespace InvenTrack.API.UnitTests;

using FluentAssertions;
using InvenTrack.API.Controllers;
using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Products.Commands.CreateProduct;
using InvenTrack.Application.Features.Products.Commands.DeleteProduct;
using InvenTrack.Application.Features.Products.Commands.UpdateProduct;
using InvenTrack.Application.Features.Products.DTOs;
using InvenTrack.Application.Features.Products.Queries.GetProductById;
using InvenTrack.Application.Features.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class ProductsControllerTests
{
    private readonly Mock<ISender> _sender = new();

    private ProductsController CreateController() => new(_sender.Object);

    [Fact]
    public async Task GetProducts_ShouldReturnOk_WithResult()
    {
        var paged = new Application.Common.Models.PaginatedList<ProductDto>(
            new List<ProductDto> { new() { Name = "Laptop" } }, 1, 1, 10);
        _sender
            .Setup(s => s.Send(It.IsAny<GetProductsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var controller = CreateController();

        var result = await controller.GetProducts(new GetProductsQuery());

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(paged);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnOk()
    {
        var dto = new ProductDto { Id = Guid.NewGuid(), Name = "Laptop" };
        _sender
            .Setup(s => s.Send(It.Is<GetProductByIdQuery>(q => q.Id == dto.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var controller = CreateController();

        var result = await controller.GetProductById(dto.Id);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(dto);
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedAtAction_WithRouteToGetById()
    {
        var dto = new ProductDto { Id = Guid.NewGuid(), Name = "Laptop" };
        _sender
            .Setup(s => s.Send(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var controller = CreateController();

        var result = await controller.Create(new CreateProductCommand("Laptop", "LAP", null, 800m, 1200m, Guid.NewGuid()));

        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(ProductsController.GetProductById));
        createdResult.RouteValues!["id"].Should().Be(dto.Id);
        createdResult.Value.Should().Be(dto);
    }

    [Fact]
    public async Task Update_ShouldReturnOk()
    {
        var dto = new ProductDto { Id = Guid.NewGuid(), Name = "Laptop" };
        _sender
            .Setup(s => s.Send(It.IsAny<UpdateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var controller = CreateController();

        var result = await controller.Update(dto.Id,
            new UpdateProductRequest("Laptop", "LAP", null, 800m, 1200m, Guid.NewGuid(), true));

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent()
    {
        _sender
            .Setup(s => s.Send(It.IsAny<DeleteProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var controller = CreateController();

        var result = await controller.Delete(Guid.NewGuid());

        result.Should().BeOfType<NoContentResult>();
    }
}
