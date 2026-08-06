namespace InvenTrack.Application.Features.Products.Queries.GetProductById;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Products.DTOs;
using MediatR;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Product with Id '{request.Id}' was not found.");

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            SKU = product.SKU,
            UnitPrice = product.UnitPrice,
            CostPrice = product.CostPrice,
            QuantityInStock = product.QuantityInStock,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? string.Empty
        };
    }
}
