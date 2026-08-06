namespace InvenTrack.Application.Features.Suppliers.Commands.CreateSupplier;

using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Suppliers.DTOs;
using InvenTrack.Domain.Entities;
using MediatR;

public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, SupplierDto>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IUnitOfWork _uow;

    public CreateSupplierCommandHandler(ISupplierRepository supplierRepository, IUnitOfWork uow)
    {
        _supplierRepository = supplierRepository;
        _uow = uow;
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
        await _uow.SaveChangesAsync();

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
