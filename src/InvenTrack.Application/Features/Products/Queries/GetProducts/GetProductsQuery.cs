namespace InvenTrack.Application.Features.Products.Queries.GetProducts;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Products.DTOs;
using AutoMapper;
using MediatR;

public class GetProductsQuery : ProductQueryParameters, IRequest<PaginatedList<ProductDto>>
{
}

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PaginatedList<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductsQueryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var result = await _productRepository.GetProductsAsync(request, cancellationToken);

        var dtos = _mapper.Map<List<ProductDto>>(result.Items);

        return new PaginatedList<ProductDto>(dtos, result.TotalCount, result.PageNumber, result.PageSize);
    }
}
