namespace InvenTrack.Application.Features.Categories.Queries.GetAllCategories;

using InvenTrack.Application.Features.Categories.DTOs;
using MediatR;

public record GetAllCategoriesQuery : IRequest<IReadOnlyList<CategoryDto>>;
