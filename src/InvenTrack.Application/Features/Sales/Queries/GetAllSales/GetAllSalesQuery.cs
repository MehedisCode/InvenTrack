namespace InvenTrack.Application.Features.Sales.Queries.GetAllSales;

using InvenTrack.Application.Features.Sales.DTOs;
using MediatR;

public record GetAllSalesQuery : IRequest<List<SaleDto>>;
