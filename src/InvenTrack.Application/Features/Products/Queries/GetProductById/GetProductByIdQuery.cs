namespace InvenTrack.Application.Features.Products.Queries.GetProductById;

using InvenTrack.Application.Features.Products.DTOs;
using MediatR;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;
