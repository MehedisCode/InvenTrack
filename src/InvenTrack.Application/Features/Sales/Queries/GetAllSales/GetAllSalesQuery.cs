namespace InvenTrack.Application.Features.Sales.Queries.GetAllSales;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Sales.DTOs;
using MediatR;

public record GetAllSalesQuery : IRequest<List<SaleDto>>;

public class GetAllSalesQueryHandler : IRequestHandler<GetAllSalesQuery, List<SaleDto>>
{
    private readonly ISaleRepository _saleRepository;

    public GetAllSalesQueryHandler(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<List<SaleDto>> Handle(GetAllSalesQuery request, CancellationToken cancellationToken)
    {
        var sales = await _saleRepository.GetAllAsync(cancellationToken);
        
        return sales.Select(s => new SaleDto
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
                UnitPrice = i.UnitPrice,
                SubTotal = i.SubTotal
            }).ToList()
        }).ToList();
    }
}
