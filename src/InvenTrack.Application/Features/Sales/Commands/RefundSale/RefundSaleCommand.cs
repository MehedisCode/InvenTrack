namespace InvenTrack.Application.Features.Sales.Commands.RefundSale;

using MediatR;

public record RefundSaleCommand(Guid SaleId) : IRequest<bool>;
