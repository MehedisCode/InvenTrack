namespace InvenTrack.Application.Features.Inventory.Commands.StockOut;

using MediatR;

public record StockOutCommand(Guid ProductId, int Quantity, string Remarks) : IRequest<Guid>;
