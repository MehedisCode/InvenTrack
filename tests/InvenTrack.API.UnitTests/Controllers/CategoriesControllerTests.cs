namespace InvenTrack.API.UnitTests;

using FluentAssertions;
using InvenTrack.API.Controllers;
using InvenTrack.Application.Features.Categories.Commands.CreateCategory;
using InvenTrack.Application.Features.Categories.Commands.DeleteCategory;
using InvenTrack.Application.Features.Categories.Commands.UpdateCategory;
using InvenTrack.Application.Features.Categories.DTOs;
using InvenTrack.Application.Features.Categories.Queries.GetAllCategories;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class CategoriesControllerTests
{
    private readonly Mock<ISender> _sender = new();

    private CategoriesController CreateController() => new(_sender.Object);

    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        var categories = new List<CategoryDto> { new() { Name = "Electronics" } };
        _sender
            .Setup(s => s.Send(It.IsAny<GetAllCategoriesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        var controller = CreateController();

        var result = await controller.GetAll();

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(categories);
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedAtAction()
    {
        var dto = new CategoryDto { Id = Guid.NewGuid(), Name = "Electronics" };
        _sender
            .Setup(s => s.Send(It.IsAny<CreateCategoryCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var controller = CreateController();

        var result = await controller.Create(new CreateCategoryCommand("Electronics", null));

        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.RouteValues!["id"].Should().Be(dto.Id);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent()
    {
        _sender
            .Setup(s => s.Send(It.IsAny<DeleteCategoryCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var controller = CreateController();

        var result = await controller.Delete(Guid.NewGuid());

        result.Should().BeOfType<NoContentResult>();
    }
}
