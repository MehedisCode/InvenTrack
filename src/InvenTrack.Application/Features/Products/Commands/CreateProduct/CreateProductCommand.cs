namespace InvenTrack.Application.Features.Products.Commands.CreateProduct;

using InvenTrack.Application.Features.Products.DTOs;
using MediatR;

public record CreateProductCommand(
    string Name,
    string SKU,
    string? Description,
    decimal PurchasePrice,
    decimal SellingPrice,
    Guid CategoryId) : IRequest<ProductDto>;

