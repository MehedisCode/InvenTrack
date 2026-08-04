namespace InvenTrack.Application.Features.Categories.Commands.CreateCategory;

using System.Threading;
using System.Threading.Tasks;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Categories.DTOs;
using InvenTrack.Domain.Entities;
using MediatR;

public record CreateCategoryCommand(string Name, string? Description) : IRequest<CategoryDto>;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Name = request.Name,
            Description = request.Description
        };

        var created = await _categoryRepository.AddAsync(category, cancellationToken);

        return new CategoryDto
        {
            Id = created.Id,
            Name = created.Name,
            Description = created.Description,
            CreatedAt = created.CreatedAt
        };
    }
}
