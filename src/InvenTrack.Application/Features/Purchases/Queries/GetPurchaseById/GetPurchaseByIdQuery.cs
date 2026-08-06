namespace InvenTrack.Application.Features.Purchases.Queries.GetPurchaseById;

using InvenTrack.Application.Features.Purchases.DTOs;
using MediatR;

public record GetPurchaseByIdQuery(Guid Id) : IRequest<PurchaseDto?>;
