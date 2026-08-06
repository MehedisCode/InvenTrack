namespace InvenTrack.Application.Features.Sales.Commands.CreateSale;

using InvenTrack.Application.Features.Sales.DTOs;
using MediatR;

public class CreateSaleCommandItem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}

public record CreateSaleCommand(
    string SaleNumber,
    string CustomerName,
    List<CreateSaleCommandItem> Items) : IRequest<SaleDto>;
