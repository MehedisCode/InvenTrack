namespace InvenTrack.Application.Features.Suppliers.Commands.UpdateSupplier;

using System;
using System.Threading;
using System.Threading.Tasks;
using InvenTrack.Application.Common.Interfaces;
using MediatR;

public record UpdateSupplierCommand(
    Guid Id,
    string CompanyName,
    string ContactPerson,
    string Phone,
    string Email) : IRequest<Unit>;

public class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand, Unit>
{
    private readonly ISupplierRepository _supplierRepository;

    public UpdateSupplierCommandHandler(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<Unit> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (supplier == null)
        {
            throw new Exception("Supplier not found.");
        }

        supplier.CompanyName = request.CompanyName;
        supplier.ContactPerson = request.ContactPerson;
        supplier.Phone = request.Phone;
        supplier.Email = request.Email;

        await _supplierRepository.UpdateAsync(supplier, cancellationToken);

        return Unit.Value;
    }
}
