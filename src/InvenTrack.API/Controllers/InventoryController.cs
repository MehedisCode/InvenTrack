namespace InvenTrack.API.Controllers;

using System.Threading.Tasks;
using InvenTrack.Application.Features.Inventory.Commands.StockIn;
using InvenTrack.Application.Features.Inventory.Commands.StockOut;
using InvenTrack.Application.Features.Inventory.Queries.GetInventoryHistory;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly ISender _sender;

    public InventoryController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Record a manual stock-in transaction (all roles).</summary>
    [HttpPost("stock-in")]
    public async Task<IActionResult> StockIn(StockInCommand command)
    {
        var result = await _sender.Send(command);
        return Ok(new { TransactionId = result });
    }

    /// <summary>Record a manual stock-out transaction (all roles).</summary>
    [HttpPost("stock-out")]
    public async Task<IActionResult> StockOut(StockOutCommand command)
    {
        var result = await _sender.Send(command);
        return Ok(new { TransactionId = result });
    }

    /// <summary>Retrieve inventory transaction history.</summary>
    /// <remarks>Supports pagination, sorting by 'date' or 'quantity', and filtering by ProductId or Date range.</remarks>
    [HttpGet("history")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistory([FromQuery] GetInventoryHistoryQuery query)
    {
        var result = await _sender.Send(query);
        return Ok(result);
    }
}
