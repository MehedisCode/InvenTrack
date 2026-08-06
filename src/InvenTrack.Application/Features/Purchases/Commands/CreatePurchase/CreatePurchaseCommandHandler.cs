namespace InvenTrack.Application.Features.Purchases.Commands.CreatePurchase;

using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Purchases.DTOs;
using InvenTrack.Domain.Entities;
using InvenTrack.Domain.Enums;
using MediatR;

public class CreatePurchaseCommandHandler : IRequestHandler<CreatePurchaseCommand, PurchaseDto>
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IProductRepository _productRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreatePurchaseCommandHandler(
        IPurchaseRepository purchaseRepository,
        IProductRepository productRepository,
        IInventoryRepository inventoryRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _purchaseRepository = purchaseRepository;
        _productRepository = productRepository;
        _inventoryRepository = inventoryRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<PurchaseDto> Handle(CreatePurchaseCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId ?? Guid.Empty;

        var purchase = new Purchase
        {
            PurchaseNumber = request.PurchaseNumber,
            SupplierId = request.SupplierId,
            UserId = userId,
            PurchaseDate = DateTime.UtcNow,
            TotalCost = 0
        };

        // Validate products and build items first (no DB writes yet for stock)
        var productUpdates = new List<(Product product, CreatePurchaseCommandItem item)>();
        foreach (var item in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
            if (product == null)
                throw new Exception($"Product with Id {item.ProductId} not found");

            var effectiveCost = product.PurchasePrice;

            var purchaseItem = new PurchaseItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitCost = effectiveCost,
                SubTotal = item.Quantity * effectiveCost
            };

            purchase.TotalCost += purchaseItem.SubTotal;
            purchase.Items.Add(purchaseItem);
            productUpdates.Add((product, item));
        }

        // Save the Purchase (and its Items) FIRST so the PurchaseId FK exists in the DB
        var createdPurchase = await _purchaseRepository.AddAsync(purchase, cancellationToken);

        // Now update stock and insert StockTransactions that reference createdPurchase.Id
        foreach (var (product, item) in productUpdates)
        {
            product.QuantityInStock += item.Quantity;
            // PurchasePrice is the source of truth — purchases read it, not write it
            await _productRepository.UpdateAsync(product, cancellationToken);

            var stockTransaction = new StockTransaction
            {
                ProductId = item.ProductId,
                UserId = userId,
                Quantity = item.Quantity,
                TransactionType = TransactionType.StockIn,
                Remarks = $"Purchase {request.PurchaseNumber}",
                PurchaseId = createdPurchase.Id
            };
            await _inventoryRepository.AddTransactionAsync(stockTransaction, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new PurchaseDto
        {
            Id = createdPurchase.Id,
            PurchaseNumber = createdPurchase.PurchaseNumber,
            SupplierId = createdPurchase.SupplierId,
            UserId = createdPurchase.UserId,
            PurchaseDate = createdPurchase.PurchaseDate,
            TotalCost = createdPurchase.TotalCost,
            Items = createdPurchase.Items.Select(i => new PurchaseItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitCost = i.UnitCost,
                SubTotal = i.SubTotal
            }).ToList()
        };
    }
}
