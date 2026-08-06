namespace InvenTrack.Application.Features.Suppliers.Queries.GetAllSuppliers;

using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Suppliers.DTOs;
using MediatR;

public class GetAllSuppliersQueryHandler : IRequestHandler<GetAllSuppliersQuery, List<SupplierDto>>
{
    private readonly ISupplierRepository _supplierRepository;

    public GetAllSuppliersQueryHandler(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<List<SupplierDto>> Handle(GetAllSuppliersQuery request, CancellationToken cancellationToken)
    {
        var suppliers = await _supplierRepository.GetAllAsync(cancellationToken);

        return suppliers.Select(s => new SupplierDto
        {
            Id = s.Id,
            CompanyName = s.CompanyName,
            ContactPerson = s.ContactPerson,
            Email = s.Email,
            Phone = s.Phone,
            Products = s.Products.Select(p => p.Name).ToList()
        }).ToList();
    }
}
