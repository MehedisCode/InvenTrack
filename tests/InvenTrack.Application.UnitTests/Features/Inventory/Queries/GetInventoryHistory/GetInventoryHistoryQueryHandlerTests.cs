namespace InvenTrack.Application.UnitTests.Features.Inventory.Queries.GetInventoryHistory;

using AutoMapper;
using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Inventory.DTOs;
using InvenTrack.Application.Features.Inventory.Queries.GetInventoryHistory;
using InvenTrack.Domain.Entities;
using Moq;
using Xunit;

public class GetInventoryHistoryQueryHandlerTests
{
    private readonly Mock<IInventoryRepository> _inventoryRepository = new();
    private readonly Mock<IMapper> _mapper = new();

    private GetInventoryHistoryQueryHandler CreateHandler() => new(_inventoryRepository.Object, _mapper.Object);

    [Fact]
    public async Task Handle_Should_ReturnPaginatedMappedTransactions()
    {
        var transactions = new List<StockTransaction>
        {
            new() { Id = Guid.NewGuid(), Quantity = 5 },
            new() { Id = Guid.NewGuid(), Quantity = 3 }
        };
        var dtos = new List<StockTransactionDto> { new(), new() };
        var paginated = new PaginatedList<StockTransaction>(transactions, 8, 1, 5);

        _inventoryRepository
            .Setup(r => r.GetTransactionsAsync(It.IsAny<TransactionQueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginated);
        _mapper
            .Setup(m => m.Map<List<StockTransactionDto>>(It.IsAny<List<StockTransaction>>()))
            .Returns(dtos);

        var handler = CreateHandler();

        var result = await handler.Handle(new GetInventoryHistoryQuery(), CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(8);
        _mapper.Verify(m => m.Map<List<StockTransactionDto>>(transactions), Times.Once);
    }
}
