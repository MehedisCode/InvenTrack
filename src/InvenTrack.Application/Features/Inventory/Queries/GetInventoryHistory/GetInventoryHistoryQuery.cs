namespace InvenTrack.Application.Features.Inventory.Queries.GetInventoryHistory;

using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Inventory.DTOs;
using MediatR;

public class GetInventoryHistoryQuery : TransactionQueryParameters, IRequest<PaginatedList<StockTransactionDto>>
{
}
