namespace InvenTrack.Application.Features.Categories.Commands.UpdateCategory;

using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Categories.DTOs;
using MediatR;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Category with Id ''{request.Id}'' was not found.");

        category.Name = request.Name;
        category.Description = request.Description;

        await _categoryRepository.UpdateAsync(category, cancellationToken);

        return new CategoryDto
        {
            Id          = category.Id,
            Name        = category.Name,
            Description = category.Description,
            CreatedAt   = category.CreatedAt
        };
    }
}
