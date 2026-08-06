namespace InvenTrack.Application.Features.Suppliers.Commands.CreateSupplier;

using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Suppliers.DTOs;
using InvenTrack.Domain.Entities;
using MediatR;

public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, SupplierDto>
{
    private readonly ISupplierRepository _supplierRepository;

    public CreateSupplierCommandHandler(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<SupplierDto> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = new Supplier
        {
            CompanyName = request.CompanyName,
            ContactPerson = request.ContactPerson,
            Phone = request.Phone,
            Email = request.Email,
            IsActive = true
        };

        var created = await _supplierRepository.AddAsync(supplier, cancellationToken);

        return new SupplierDto
        {
            Id = created.Id,
            CompanyName = created.CompanyName,
            ContactPerson = created.ContactPerson,
            Phone = created.Phone,
            Email = created.Email
        };
    }
}
