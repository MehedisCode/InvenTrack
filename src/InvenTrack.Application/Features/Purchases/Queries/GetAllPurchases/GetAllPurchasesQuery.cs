namespace InvenTrack.Application.Features.Purchases.Queries.GetAllPurchases;

using InvenTrack.Application.Features.Purchases.DTOs;
using MediatR;

public record GetAllPurchasesQuery : IRequest<List<PurchaseDto>>;
