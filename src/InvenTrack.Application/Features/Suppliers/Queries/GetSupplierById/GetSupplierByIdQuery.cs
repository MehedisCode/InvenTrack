namespace InvenTrack.Application.Features.Suppliers.Queries.GetSupplierById;

using InvenTrack.Application.Features.Suppliers.DTOs;
using MediatR;

public record GetSupplierByIdQuery(Guid Id) : IRequest<SupplierDto?>;
