namespace InvenTrack.Application.Features.Sales.Queries.GetSaleById;

using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Sales.DTOs;
using MediatR;

public class GetSaleByIdQueryHandler : IRequestHandler<GetSaleByIdQuery, SaleDto?>
{
    private readonly ISaleRepository _saleRepository;

    public GetSaleByIdQueryHandler(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<SaleDto?> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);

        if (sale == null)
            return null;

        return new SaleDto
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            CustomerName = sale.CustomerName,
            UserId = sale.UserId,
            SaleDate = sale.SaleDate,
            TotalAmount = sale.TotalAmount,
            Items = sale.Items.Select(i => new SaleItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                SubTotal = i.SubTotal
            }).ToList()
        };
    }
}
