namespace InvenTrack.Application.Features.Suppliers.Queries.SearchSuppliers;

using InvenTrack.Application.Features.Suppliers.DTOs;
using MediatR;

public record SearchSuppliersQuery(string Term) : IRequest<List<SupplierDto>>;
