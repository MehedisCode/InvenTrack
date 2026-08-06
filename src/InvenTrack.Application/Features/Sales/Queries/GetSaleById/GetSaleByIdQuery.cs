namespace InvenTrack.Application.Features.Sales.Queries.GetSaleById;

using InvenTrack.Application.Features.Sales.DTOs;
using MediatR;

public record GetSaleByIdQuery(Guid Id) : IRequest<SaleDto?>;
