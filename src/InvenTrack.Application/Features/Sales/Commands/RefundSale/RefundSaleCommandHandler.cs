namespace InvenTrack.Application.Features.Sales.Commands.RefundSale;

using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Domain.Entities;
using InvenTrack.Domain.Enums;
using MediatR;

public class RefundSaleCommandHandler : IRequestHandler<RefundSaleCommand, bool>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public RefundSaleCommandHandler(
        ISaleRepository saleRepository,
        IProductRepository productRepository,
        IInventoryRepository inventoryRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
        _inventoryRepository = inventoryRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(RefundSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (sale == null)
            throw new Exception($"Sale with Id {request.SaleId} not found");

        var userId = _currentUserService.UserId ?? Guid.Empty;

        foreach (var item in sale.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
            if (product != null)
            {
                // Refund stock
                product.QuantityInStock += item.Quantity;
                await _productRepository.UpdateAsync(product, cancellationToken);

                // Create stock transaction for stock adjustment (refund)
                var stockTransaction = new StockTransaction
                {
                    ProductId = item.ProductId,
                    UserId = userId,
                    Quantity = item.Quantity,
                    TransactionType = TransactionType.StockIn,
                    Remarks = $"Refund for Sale {sale.SaleNumber}",
                    SaleId = sale.Id
                };
                await _inventoryRepository.AddTransactionAsync(stockTransaction, cancellationToken);
            }
        }

        // A complete refund could involve marking the sale as refunded,
        // but for now we simply revert the stock and return true.
        // Further domain changes would be needed for sale status tracking.

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
