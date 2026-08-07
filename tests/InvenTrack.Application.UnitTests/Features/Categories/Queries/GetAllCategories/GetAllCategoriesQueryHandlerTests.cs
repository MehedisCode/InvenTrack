namespace InvenTrack.Application.UnitTests.Features.Categories.Queries.GetAllCategories;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
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
            .Setup(r => r.GetAllCategoriesAsync(It.IsAny<CategoryQueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedList<Category>(categories, categories.Count, 1, 10));

        var handler = CreateHandler();

        var result = await handler.Handle(new GetAllCategoriesQuery(), CancellationToken.None);

        result.Items.Should().HaveCount(2);
        result.Items.Select(c => c.Name).Should().ContainInOrder("Electronics", "Books");
        result.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyList_WhenNoCategories()
    {
        _categoryRepository
            .Setup(r => r.GetAllCategoriesAsync(It.IsAny<CategoryQueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedList<Category>(new List<Category>(), 0, 1, 10));

        var handler = CreateHandler();

        var result = await handler.Handle(new GetAllCategoriesQuery(), CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}
