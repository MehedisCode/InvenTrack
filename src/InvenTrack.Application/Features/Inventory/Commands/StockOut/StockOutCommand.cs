namespace InvenTrack.Application.Features.Inventory.Commands.StockOut;

using System;
using System.Threading;
using System.Threading.Tasks;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Domain.Entities;
using InvenTrack.Domain.Enums;
using MediatR;

public record StockOutCommand(Guid ProductId, int Quantity, string Remarks) : IRequest<Guid>;

public class StockOutCommandHandler : IRequestHandler<StockOutCommand, Guid>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICurrentUserService _currentUserService;

    public StockOutCommandHandler(
        IInventoryRepository inventoryRepository,
        IProductRepository productRepository,
        ICurrentUserService currentUserService)
    {
        _inventoryRepository = inventoryRepository;
        _productRepository = productRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(StockOutCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null)
            throw new Exception("Product not found");

        if (product.QuantityInStock < request.Quantity)
            throw new Exception("Insufficient stock for transaction.");

        product.QuantityInStock -= request.Quantity;
        await _productRepository.UpdateAsync(product, cancellationToken);

        var userId = _currentUserService.UserId ?? Guid.Empty;

        var transaction = new StockTransaction
        {
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            TransactionType = TransactionType.StockOut,
            Remarks = request.Remarks,
            UserId = userId
        };

        var created = await _inventoryRepository.AddTransactionAsync(transaction, cancellationToken);
        return created.Id;
    }
}
