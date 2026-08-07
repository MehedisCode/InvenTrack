namespace InvenTrack.API.UnitTests;

using FluentAssertions;
using InvenTrack.API.Controllers;
using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Inventory.Commands.StockIn;
using InvenTrack.Application.Features.Inventory.Commands.StockOut;
using InvenTrack.Application.Features.Inventory.DTOs;
using InvenTrack.Application.Features.Inventory.Queries.GetInventoryHistory;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class InventoryControllerTests
{
    private readonly Mock<ISender> _sender = new();

    private InventoryController CreateController() => new(_sender.Object);

    [Fact]
    public async Task StockIn_ShouldReturnOk_WithTransactionId()
    {
        var transactionId = Guid.NewGuid();
        _sender
            .Setup(s => s.Send(It.IsAny<StockInCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactionId);

        var controller = CreateController();

        var result = await controller.StockIn(new StockInCommand(Guid.NewGuid(), 5, "Restock"));

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(new { TransactionId = transactionId });
    }

    [Fact]
    public async Task StockOut_ShouldReturnOk_WithTransactionId()
    {
        var transactionId = Guid.NewGuid();
        _sender
            .Setup(s => s.Send(It.IsAny<StockOutCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactionId);

        var controller = CreateController();

        var result = await controller.StockOut(new StockOutCommand(Guid.NewGuid(), 2, "Adjustment"));

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(new { TransactionId = transactionId });
    }

    [Fact]
    public async Task GetHistory_ShouldReturnOk_WithResult()
    {
        var paged = new PaginatedList<StockTransactionDto>(
            new List<StockTransactionDto>(), 0, 1, 10);
        _sender
            .Setup(s => s.Send(It.IsAny<GetInventoryHistoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var controller = CreateController();

        var result = await controller.GetHistory(new GetInventoryHistoryQuery());

        result.Should().BeOfType<OkObjectResult>();
    }
}
