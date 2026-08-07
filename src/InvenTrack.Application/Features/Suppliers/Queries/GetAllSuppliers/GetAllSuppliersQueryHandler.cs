namespace InvenTrack.Application.Features.Suppliers.Queries.GetAllSuppliers;

using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Suppliers.DTOs;
using MediatR;

public class GetAllSuppliersQueryHandler : IRequestHandler<GetAllSuppliersQuery, PaginatedList<SupplierDto>>
{
    private readonly ISupplierRepository _supplierRepository;

    public GetAllSuppliersQueryHandler(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<PaginatedList<SupplierDto>> Handle(GetAllSuppliersQuery request, CancellationToken cancellationToken)
    {
        var suppliers = await _supplierRepository.GetAllSuppliersAsync(request, cancellationToken);

        var dtos = suppliers.Items.Select(s => new SupplierDto
        {
            Id = s.Id,
            CompanyName = s.CompanyName,
            ContactPerson = s.ContactPerson,
            Email = s.Email,
            Phone = s.Phone,
            Products = s.Products.Select(p => p.Name).ToList()
        }).ToList();

        return new PaginatedList<SupplierDto>(dtos, suppliers.TotalCount, suppliers.PageNumber, suppliers.PageSize);
    }
}
