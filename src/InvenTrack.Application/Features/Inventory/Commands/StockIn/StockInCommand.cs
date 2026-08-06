namespace InvenTrack.Application.Features.Inventory.Commands.StockIn;

using MediatR;

public record StockInCommand(Guid ProductId, int Quantity, string Remarks) : IRequest<Guid>;
