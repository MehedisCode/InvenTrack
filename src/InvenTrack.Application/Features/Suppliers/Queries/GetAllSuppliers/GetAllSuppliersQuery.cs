namespace InvenTrack.Application.Features.Suppliers.Queries.GetAllSuppliers;

using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Suppliers.DTOs;
using MediatR;

public class GetAllSuppliersQuery : SupplierQueryParameters, IRequest<PaginatedList<SupplierDto>>
{
}
