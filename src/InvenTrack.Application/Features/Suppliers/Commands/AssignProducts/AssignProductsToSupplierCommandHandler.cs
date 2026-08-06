namespace InvenTrack.Application.Features.Suppliers.Commands.AssignProducts;

using InvenTrack.Application.Common.Interfaces;
using MediatR;

public class AssignProductsToSupplierCommandHandler : IRequestHandler<AssignProductsToSupplierCommand, Unit>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignProductsToSupplierCommandHandler(
        ISupplierRepository supplierRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _supplierRepository = supplierRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(AssignProductsToSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdWithProductsAsync(request.SupplierId, cancellationToken);

        if (supplier == null)
        {
            throw new Exception($"Supplier with ID {request.SupplierId} was not found.");
        }

        foreach (var productId in request.ProductIds)
        {
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);

            if (product == null)
            {
                throw new Exception($"Product with ID {productId} was not found.");
            }

            // Only add if not already linked
            if (!supplier.Products.Any(p => p.Id == productId))
            {
                supplier.Products.Add(product);
            }
        }

        await _supplierRepository.UpdateAsync(supplier, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
