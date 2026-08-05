namespace InvenTrack.Application.Features.Sales.Commands.CreateSale;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Sales.DTOs;
using InvenTrack.Domain.Entities;
using InvenTrack.Domain.Enums;
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

public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, SaleDto>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateSaleCommandHandler(
        ISaleRepository saleRepository,
        IProductRepository productRepository,
        IInventoryRepository inventoryRepository,
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
        _inventoryRepository = inventoryRepository;
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<SaleDto> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var userId = _currentUserService.UserId ?? Guid.Empty;
            
            var sale = new Sale
            {
                SaleNumber = request.SaleNumber,
                CustomerName = request.CustomerName,
                UserId = userId,
                SaleDate = DateTime.UtcNow,
                TotalAmount = 0
            };

            foreach (var item in request.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
                if (product == null)
                    throw new Exception($"Product with Id {item.ProductId} not found");

                if (product.QuantityInStock < item.Quantity)
                    throw new Exception($"Insufficient stock for product {product.Name}. Available: {product.QuantityInStock}, Requested: {item.Quantity}");

                var saleItem = new SaleItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.UnitPrice, // snapshot UnitPrice
                    SubTotal = item.Quantity * product.UnitPrice
                };
                
                sale.TotalAmount += saleItem.SubTotal;
                sale.Items.Add(saleItem);

                // Deduct product stock
                product.QuantityInStock -= item.Quantity;
                await _productRepository.UpdateAsync(product, cancellationToken);

                // Create stock transaction
                var stockTransaction = new StockTransaction
                {
                    ProductId = item.ProductId,
                    UserId = userId,
                    Quantity = item.Quantity,
                    TransactionType = TransactionType.StockOut,
                    Remarks = $"Sale {request.SaleNumber}",
                    Sale = sale
                };
                await _inventoryRepository.AddTransactionAsync(stockTransaction, cancellationToken);
            }

            var createdSale = await _saleRepository.AddAsync(sale, cancellationToken);
            
            await _context.Database.CommitTransactionAsync(cancellationToken);

            return new SaleDto
            {
                Id = createdSale.Id,
                SaleNumber = createdSale.SaleNumber,
                CustomerName = createdSale.CustomerName,
                UserId = createdSale.UserId,
                SaleDate = createdSale.SaleDate,
                TotalAmount = createdSale.TotalAmount,
                Items = createdSale.Items.Select(i => new SaleItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    SubTotal = i.SubTotal
                }).ToList()
            };
        }
        catch (Exception)
        {
            await _context.Database.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
