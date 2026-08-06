namespace InvenTrack.Application.Features.Products.Queries.GetProductById;

using AutoMapper;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Products.DTOs;
using MediatR;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Product with Id '{request.Id}' was not found.");

        return _mapper.Map<ProductDto>(product);
    }
}
