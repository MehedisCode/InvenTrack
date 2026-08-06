namespace InvenTrack.Application.Features.Categories.Commands.CreateCategory;

using InvenTrack.Application.Features.Categories.DTOs;
using MediatR;

public record CreateCategoryCommand(string Name, string? Description) : IRequest<CategoryDto>;
