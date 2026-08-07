namespace InvenTrack.Application.UnitTests.Features.Categories.Commands.UpdateCategory;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Categories.Commands.UpdateCategory;
using InvenTrack.Domain.Entities;
using Moq;
using Xunit;

public class UpdateCategoryCommandHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private UpdateCategoryCommandHandler CreateHandler() => new(_categoryRepository.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_Should_UpdateCategory_WhenCategoryExists()
    {
        var categoryId = Guid.NewGuid();
        var existing = new Category { Id = categoryId, Name = "Old", Description = "Old desc" };

        _categoryRepository
            .Setup(r => r.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var handler = CreateHandler();

        var result = await handler.Handle(new UpdateCategoryCommand(categoryId, "New", "New desc"), CancellationToken.None);

        result.Name.Should().Be("New");
        result.Description.Should().Be("New desc");
        _categoryRepository.Verify(r => r.UpdateAsync(It.Is<Category>(c =>
            c.Id == categoryId && c.Name == "New" && c.Description == "New desc"), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowKeyNotFoundException_WhenCategoryDoesNotExist()
    {
        var categoryId = Guid.NewGuid();
        _categoryRepository
            .Setup(r => r.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var handler = CreateHandler();

        await FluentActions.Invoking(() => handler.Handle(new UpdateCategoryCommand(categoryId, "New", "desc"), CancellationToken.None))
            .Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*'{categoryId}'*");

        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
