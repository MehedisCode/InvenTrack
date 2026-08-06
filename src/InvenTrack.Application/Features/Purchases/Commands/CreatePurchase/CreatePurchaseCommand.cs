namespace InvenTrack.Application.Features.Purchases.Commands.CreatePurchase;

using InvenTrack.Application.Features.Purchases.DTOs;
using MediatR;

public class CreatePurchaseCommandItem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}

public record CreatePurchaseCommand(
    string PurchaseNumber,
    Guid SupplierId,
    List<CreatePurchaseCommandItem> Items) : IRequest<PurchaseDto>;
