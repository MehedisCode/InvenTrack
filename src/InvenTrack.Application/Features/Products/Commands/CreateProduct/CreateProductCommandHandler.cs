namespace InvenTrack.Application.Features.Products.Commands.CreateProduct;

using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Products.DTOs;
using InvenTrack.Domain.Entities;
using MediatR;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            SKU = request.SKU,
            Description = request.Description,
            PurchasePrice = request.PurchasePrice,
            SellingPrice = request.SellingPrice,
            QuantityInStock = 0, // Initial quantity is 0, must use Stock In
            CategoryId = request.CategoryId,
            IsActive = true
        };

        var created = await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ProductDto
        {
            Id = created.Id,
            Name = created.Name,
            SKU = created.SKU,
            PurchasePrice = created.PurchasePrice,
            SellingPrice = created.SellingPrice,
            QuantityInStock = created.QuantityInStock,
            CategoryId = created.CategoryId
        };
    }
}
