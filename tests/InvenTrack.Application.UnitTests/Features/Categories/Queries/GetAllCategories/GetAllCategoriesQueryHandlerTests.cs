namespace InvenTrack.Application.UnitTests.Features.Categories.Queries.GetAllCategories;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Categories.Queries.GetAllCategories;
using InvenTrack.Domain.Entities;
using Moq;
using Xunit;

public class GetAllCategoriesQueryHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepository = new();

    private GetAllCategoriesQueryHandler CreateHandler() => new(_categoryRepository.Object);

    [Fact]
    public async Task Handle_Should_ReturnAllCategories_AsDtos()
    {
        var categories = new List<Category>
        {
            new() { Id = Guid.NewGuid(), Name = "Electronics", Description = "Devices" },
            new() { Id = Guid.NewGuid(), Name = "Books", Description = "Reading" }
        };

        _categoryRepository
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        var handler = CreateHandler();

        var result = await handler.Handle(new GetAllCategoriesQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
        result.Select(c => c.Name).Should().ContainInOrder("Electronics", "Books");
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyList_WhenNoCategories()
    {
        _categoryRepository
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Category>());

        var handler = CreateHandler();

        var result = await handler.Handle(new GetAllCategoriesQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
