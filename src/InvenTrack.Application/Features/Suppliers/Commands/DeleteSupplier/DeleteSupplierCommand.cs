namespace InvenTrack.Application.Features.Suppliers.Commands.DeleteSupplier;

using System;
using System.Threading;
using System.Threading.Tasks;
using InvenTrack.Application.Common.Interfaces;
using MediatR;

public record DeleteSupplierCommand(Guid Id) : IRequest<Unit>;

public class DeleteSupplierCommandHandler : IRequestHandler<DeleteSupplierCommand, Unit>
{
    private readonly ISupplierRepository _supplierRepository;

    public DeleteSupplierCommandHandler(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<Unit> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (supplier == null)
        {
            throw new Exception("Supplier not found.");
        }

        // Soft Delete
        supplier.IsActive = false;
        
        await _supplierRepository.UpdateAsync(supplier, cancellationToken);

        return Unit.Value;
    }
}
