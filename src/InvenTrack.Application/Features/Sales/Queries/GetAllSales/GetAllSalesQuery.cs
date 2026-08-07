namespace InvenTrack.Application.Features.Sales.Queries.GetAllSales;

using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Sales.DTOs;
using MediatR;

public class GetAllSalesQuery : SaleQueryParameters, IRequest<PaginatedList<SaleDto>>
{
}
