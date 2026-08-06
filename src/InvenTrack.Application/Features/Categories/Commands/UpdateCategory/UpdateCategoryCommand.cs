namespace InvenTrack.Application.Features.Categories.Commands.UpdateCategory;

using InvenTrack.Application.Features.Categories.DTOs;
using MediatR;

public record UpdateCategoryCommand(Guid Id, string Name, string? Description) : IRequest<CategoryDto>;
