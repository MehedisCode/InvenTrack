namespace InvenTrack.Application.Features.Products.Commands.UpdateProduct;

using InvenTrack.Application.Features.Products.DTOs;
using MediatR;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string SKU,
    string? Description,
    decimal UnitPrice,
    decimal CostPrice,
    Guid CategoryId,
    bool IsActive) : IRequest<ProductDto>;
