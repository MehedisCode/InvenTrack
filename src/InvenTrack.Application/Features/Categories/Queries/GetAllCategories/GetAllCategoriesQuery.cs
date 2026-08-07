namespace InvenTrack.Application.Features.Categories.Queries.GetAllCategories;

using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Categories.DTOs;
using MediatR;

public class GetAllCategoriesQuery : CategoryQueryParameters, IRequest<PaginatedList<CategoryDto>>
{
}
