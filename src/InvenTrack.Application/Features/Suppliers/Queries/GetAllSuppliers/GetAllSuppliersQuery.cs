namespace InvenTrack.Application.Features.Suppliers.Queries.GetAllSuppliers;

using InvenTrack.Application.Features.Suppliers.DTOs;
using MediatR;

public record GetAllSuppliersQuery() : IRequest<List<SupplierDto>>;
