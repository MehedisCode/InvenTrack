namespace InvenTrack.Application.Features.Suppliers.Queries.SearchSuppliers;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Suppliers.DTOs;
using MediatR;

public record SearchSuppliersQuery(string Term) : IRequest<List<SupplierDto>>;

public class SearchSuppliersQueryHandler : IRequestHandler<SearchSuppliersQuery, List<SupplierDto>>
{
    private readonly ISupplierRepository _supplierRepository;

    public SearchSuppliersQueryHandler(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<List<SupplierDto>> Handle(SearchSuppliersQuery request, CancellationToken cancellationToken)
    {
        var suppliers = await _supplierRepository.SearchAsync(request.Term, cancellationToken);
        
        return suppliers.Select(s => new SupplierDto
        {
            Id = s.Id,
            CompanyName = s.CompanyName,
            ContactPerson = s.ContactPerson,
            Email = s.Email,
            Phone = s.Phone
        }).ToList();
    }
}
