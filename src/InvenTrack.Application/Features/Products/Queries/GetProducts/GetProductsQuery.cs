namespace InvenTrack.Application.Features.Products.Queries.GetProducts;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Products.DTOs;
using MediatR;

public class GetProductsQuery : ProductQueryParameters, IRequest<PaginatedList<ProductDto>>
{
}

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PaginatedList<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public GetProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PaginatedList<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var result = await _productRepository.GetProductsAsync(request, cancellationToken);
        
        var dtos = result.Items.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            SKU = p.SKU,
            UnitPrice = p.UnitPrice,
            QuantityInStock = p.QuantityInStock,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name ?? string.Empty
        }).ToList();

        return new PaginatedList<ProductDto>(dtos, result.TotalCount, result.PageNumber, result.PageSize);
    }
}
