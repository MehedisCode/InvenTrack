namespace InvenTrack.Application.Features.Sales.Queries.GetAllSales;

using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Sales.DTOs;
using MediatR;

public class GetAllSalesQueryHandler : IRequestHandler<GetAllSalesQuery, PaginatedList<SaleDto>>
{
    private readonly ISaleRepository _saleRepository;

    public GetAllSalesQueryHandler(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<PaginatedList<SaleDto>> Handle(GetAllSalesQuery request, CancellationToken cancellationToken)
    {
        var sales = await _saleRepository.GetAllSalesAsync(request, cancellationToken);

        var dtos = sales.Items.Select(s => new SaleDto
        {
            Id = s.Id,
            SaleNumber = s.SaleNumber,
            CustomerName = s.CustomerName,
            UserId = s.UserId,
            SaleDate = s.SaleDate,
            TotalAmount = s.TotalAmount,
            Items = s.Items.Select(i => new SaleItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                SellingPrice = i.SellingPrice,
                SubTotal = i.SubTotal
            }).ToList()
        }).ToList();

        return new PaginatedList<SaleDto>(dtos, sales.TotalCount, sales.PageNumber, sales.PageSize);
    }
}
