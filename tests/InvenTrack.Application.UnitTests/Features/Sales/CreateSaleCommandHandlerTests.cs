namespace InvenTrack.Application.UnitTests.Features.Sales;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Sales.Commands.CreateSale;
using InvenTrack.Domain.Entities;
using Moq;

public class CreateSaleCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldPersistSaleAndRelatedEntitiesOnceThroughUnitOfWork()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        var saleRepository = new Mock<ISaleRepository>();
        var productRepository = new Mock<IProductRepository>();
        var inventoryRepository = new Mock<IInventoryRepository>();
        var currentUserService = new Mock<ICurrentUserService>();

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Widget",
            QuantityInStock = 10,
            SellingPrice = 5m
        };

        productRepository
            .Setup(repository => repository.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        saleRepository
            .Setup(repository => repository.AddAsync(It.IsAny<Sale>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sale sale, CancellationToken _) => sale);

        inventoryRepository
            .Setup(repository => repository.AddTransactionAsync(It.IsAny<StockTransaction>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((StockTransaction transaction, CancellationToken _) => transaction);

        currentUserService.SetupGet(service => service.UserId).Returns(Guid.NewGuid());

        var handler = new CreateSaleCommandHandler(
            saleRepository.Object,
            productRepository.Object,
            inventoryRepository.Object,
            unitOfWork.Object,
            currentUserService.Object);

        var request = new CreateSaleCommand(
            "S-1001",
            "Alice",
            new List<CreateSaleCommandItem>
            {
                new() { ProductId = product.Id, Quantity = 2 }
            });

        var result = await handler.Handle(request, CancellationToken.None);

        unitOfWork.Verify(work => work.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(work => work.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        saleRepository.Verify(repository => repository.AddAsync(It.IsAny<Sale>(), It.IsAny<CancellationToken>()), Times.Once);
        productRepository.Verify(repository => repository.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        inventoryRepository.Verify(repository => repository.AddTransactionAsync(It.IsAny<StockTransaction>(), It.IsAny<CancellationToken>()), Times.Once);
        result.TotalAmount.Should().Be(10m);
    }
}
