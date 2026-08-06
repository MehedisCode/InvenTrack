namespace InvenTrack.Application.Features.Products.Commands.UpdateProduct;

using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Products.DTOs;
using MediatR;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Product with Id '{request.Id}' was not found.");

        product.Name = request.Name;
        product.SKU = request.SKU;
        product.Description = request.Description;
        product.PurchasePrice = request.PurchasePrice;
        product.SellingPrice = request.SellingPrice;
        product.CategoryId = request.CategoryId;
        product.IsActive = request.IsActive;

        await _productRepository.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            SKU = product.SKU,
            PurchasePrice = product.PurchasePrice,
            SellingPrice = product.SellingPrice,
            QuantityInStock = product.QuantityInStock,
            CategoryId = product.CategoryId
        };
    }
}
