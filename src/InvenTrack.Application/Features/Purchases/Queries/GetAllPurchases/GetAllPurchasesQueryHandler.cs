namespace InvenTrack.Application.Features.Purchases.Queries.GetAllPurchases;

using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Purchases.DTOs;
using MediatR;

public class GetAllPurchasesQueryHandler : IRequestHandler<GetAllPurchasesQuery, PaginatedList<PurchaseDto>>
{
    private readonly IPurchaseRepository _purchaseRepository;

    public GetAllPurchasesQueryHandler(IPurchaseRepository purchaseRepository)
    {
        _purchaseRepository = purchaseRepository;
    }

    public async Task<PaginatedList<PurchaseDto>> Handle(GetAllPurchasesQuery request, CancellationToken cancellationToken)
    {
        var purchases = await _purchaseRepository.GetAllPurchasesAsync(request, cancellationToken);

        var dtos = purchases.Items.Select(p => new PurchaseDto
        {
            Id = p.Id,
            PurchaseNumber = p.PurchaseNumber,
            SupplierId = p.SupplierId,
            UserId = p.UserId,
            PurchaseDate = p.PurchaseDate,
            TotalCost = p.TotalCost,
            Items = p.Items.Select(i => new PurchaseItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitCost = i.UnitCost,
                SubTotal = i.SubTotal
            }).ToList()
        }).ToList();

        return new PaginatedList<PurchaseDto>(dtos, purchases.TotalCount, purchases.PageNumber, purchases.PageSize);
    }
}
