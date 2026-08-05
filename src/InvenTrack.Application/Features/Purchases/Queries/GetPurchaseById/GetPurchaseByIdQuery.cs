namespace InvenTrack.Application.Features.Purchases.Queries.GetPurchaseById;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Purchases.DTOs;
using MediatR;

public record GetPurchaseByIdQuery(Guid Id) : IRequest<PurchaseDto?>;

public class GetPurchaseByIdQueryHandler : IRequestHandler<GetPurchaseByIdQuery, PurchaseDto?>
{
    private readonly IPurchaseRepository _purchaseRepository;

    public GetPurchaseByIdQueryHandler(IPurchaseRepository purchaseRepository)
    {
        _purchaseRepository = purchaseRepository;
    }

    public async Task<PurchaseDto?> Handle(GetPurchaseByIdQuery request, CancellationToken cancellationToken)
    {
        var purchase = await _purchaseRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (purchase == null)
            return null;

        return new PurchaseDto
        {
            Id = purchase.Id,
            PurchaseNumber = purchase.PurchaseNumber,
            SupplierId = purchase.SupplierId,
            UserId = purchase.UserId,
            PurchaseDate = purchase.PurchaseDate,
            TotalCost = purchase.TotalCost,
            Items = purchase.Items.Select(i => new PurchaseItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitCost = i.UnitCost,
                SubTotal = i.SubTotal
            }).ToList()
        };
    }
}
