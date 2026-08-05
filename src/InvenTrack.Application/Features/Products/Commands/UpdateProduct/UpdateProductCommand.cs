namespace InvenTrack.Application.Features.Products.Commands.UpdateProduct;

using System;
using System.Threading;
using System.Threading.Tasks;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Products.DTOs;
using MediatR;

public record UpdateProductCommand(
    Guid    Id,
    string  Name,
    string  SKU,
    string? Description,
    decimal UnitPrice,
    decimal CostPrice,
    Guid    CategoryId,
    bool    IsActive) : IRequest<ProductDto>;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;

    public UpdateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Product with Id '{request.Id}' was not found.");

        product.Name        = request.Name;
        product.SKU         = request.SKU;
        product.Description = request.Description;
        product.UnitPrice   = request.UnitPrice;
        product.CostPrice   = request.CostPrice;
        product.CategoryId  = request.CategoryId;
        product.IsActive    = request.IsActive;

        await _productRepository.UpdateAsync(product, cancellationToken);

        return new ProductDto
        {
            Id              = product.Id,
            Name            = product.Name,
            SKU             = product.SKU,
            UnitPrice       = product.UnitPrice,
            QuantityInStock = product.QuantityInStock,
            CategoryId      = product.CategoryId
        };
    }
}
