namespace InvenTrack.Application.UnitTests.Features.Categories.Commands.CreateCategory;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Categories.Commands.CreateCategory;
using InvenTrack.Domain.Entities;
using Moq;
using Xunit;

public class CreateCategoryCommandHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private CreateCategoryCommandHandler CreateHandler() => new(_categoryRepository.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_Should_CreateCategory_When_RequestIsValid()
    {
        _categoryRepository
            .Setup(r => r.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category c, CancellationToken _) => c);

        var handler = CreateHandler();
        var command = new CreateCategoryCommand("Electronics", "Devices and gadgets");

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be("Electronics");
        result.Description.Should().Be("Devices and gadgets");
        _categoryRepository.Verify(r => r.AddAsync(It.Is<Category>(c =>
            c.Name == "Electronics" && c.Description == "Devices and gadgets"), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_AllowNullDescription()
    {
        _categoryRepository
            .Setup(r => r.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category c, CancellationToken _) => c);

        var handler = CreateHandler();

        var result = await handler.Handle(new CreateCategoryCommand("Books", null), CancellationToken.None);

        result.Description.Should().BeNull();
    }
}
