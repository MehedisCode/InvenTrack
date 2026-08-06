namespace InvenTrack.Application.Features.Sales.Commands.CreateSale;

using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Sales.DTOs;
using InvenTrack.Domain.Entities;
using InvenTrack.Domain.Enums;
using MediatR;

public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, SaleDto>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateSaleCommandHandler(
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

    public async Task<SaleDto> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            if (_currentUserService.UserId is null)
            {
                throw new UnauthorizedAccessException("Current user could not be determined.");
            }

            var userId = _currentUserService.UserId.Value;

            var sale = new Sale
            {
                SaleNumber = request.SaleNumber,
                CustomerName = request.CustomerName,
                UserId = userId,
                SaleDate = DateTime.UtcNow
            };

            foreach (var item in request.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);

                if (product is null)
                {
                    throw new InvalidOperationException($"Product '{item.ProductId}' was not found.");
                }

                if (product.QuantityInStock < item.Quantity)
                {
                    throw new InvalidOperationException(
                        $"Insufficient stock for product '{product.Name}'.");
                }

                var saleItem = new SaleItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    SellingPrice = product.SellingPrice,
                    SubTotal = item.Quantity * product.SellingPrice
                };

                sale.TotalAmount += saleItem.SubTotal;
                sale.Items.Add(saleItem);

                product.QuantityInStock -= item.Quantity;
                await _productRepository.UpdateAsync(product, cancellationToken);

                await _inventoryRepository.AddTransactionAsync(
                    new StockTransaction
                    {
                        ProductId = item.ProductId,
                        UserId = userId,
                        Quantity = item.Quantity,
                        TransactionType = TransactionType.StockOut,
                        Remarks = $"Sale {request.SaleNumber}",
                        Sale = sale
                    },
                    cancellationToken);
            }

            await _saleRepository.AddAsync(sale, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

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
                    SellingPrice = i.SellingPrice,
                    SubTotal = i.SubTotal
                }).ToList()
            };
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}