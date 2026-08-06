namespace InvenTrack.Application.Features.Products.Queries.GetProducts;

using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Products.DTOs;
using MediatR;

public class GetProductsQuery : ProductQueryParameters, IRequest<PaginatedList<ProductDto>>
{
}
