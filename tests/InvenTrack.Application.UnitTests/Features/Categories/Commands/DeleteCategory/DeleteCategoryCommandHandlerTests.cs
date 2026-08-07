namespace InvenTrack.Application.UnitTests.Features.Categories.Commands.DeleteCategory;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Categories.Commands.DeleteCategory;
using InvenTrack.Domain.Entities;
using Moq;
using Xunit;

public class DeleteCategoryCommandHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private DeleteCategoryCommandHandler CreateHandler() => new(_categoryRepository.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_Should_DeleteCategory_WhenCategoryExists()
    {
        var categoryId = Guid.NewGuid();
        var existing = new Category { Id = categoryId, Name = "Electronics" };

        _categoryRepository
            .Setup(r => r.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var handler = CreateHandler();

        await handler.Handle(new DeleteCategoryCommand(categoryId), CancellationToken.None);

        _categoryRepository.Verify(r => r.DeleteAsync(It.Is<Category>(c => c.Id == categoryId), It.IsAny<CancellationToken>()), Times.Once);
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

        await FluentActions.Invoking(() => handler.Handle(new DeleteCategoryCommand(categoryId), CancellationToken.None))
            .Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*'{categoryId}'*");

        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
