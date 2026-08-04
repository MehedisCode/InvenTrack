namespace InvenTrack.Application.Features.Products.Commands.CreateProduct;

using System;
using System.Threading;
using System.Threading.Tasks;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Products.DTOs;
using InvenTrack.Domain.Entities;
using MediatR;

public record CreateProductCommand(
    string Name,
    string SKU,
    string? Description,
    decimal UnitPrice,
    Guid CategoryId,
    Guid SupplierId) : IRequest<ProductDto>;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;

    public CreateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            SKU = request.SKU,
            Description = request.Description,
            UnitPrice = request.UnitPrice,
            QuantityInStock = 0, // Initial quantity is 0, must use Stock In
            CategoryId = request.CategoryId,
            SupplierId = request.SupplierId,
            IsActive = true
        };

        var created = await _productRepository.AddAsync(product, cancellationToken);

        return new ProductDto
        {
            Id = created.Id,
            Name = created.Name,
            SKU = created.SKU,
            UnitPrice = created.UnitPrice,
            QuantityInStock = created.QuantityInStock,
            CategoryId = created.CategoryId,
            SupplierId = created.SupplierId
        };
    }
}
