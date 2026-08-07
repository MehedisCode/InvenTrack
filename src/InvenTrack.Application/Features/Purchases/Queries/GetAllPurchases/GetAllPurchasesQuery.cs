namespace InvenTrack.Application.Features.Purchases.Queries.GetAllPurchases;

using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Purchases.DTOs;
using MediatR;

public class GetAllPurchasesQuery : PurchaseQueryParameters, IRequest<PaginatedList<PurchaseDto>>
{
}
