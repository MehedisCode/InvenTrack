namespace InvenTrack.API.Controllers;

using System.Threading.Tasks;
using InvenTrack.Application.Features.Inventory.Commands.StockIn;
using InvenTrack.Application.Features.Inventory.Commands.StockOut;
using MediatR;
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

    [HttpPost("stock-in")]
    public async Task<IActionResult> StockIn(StockInCommand command)
    {
        var result = await _sender.Send(command);
        return Ok(new { TransactionId = result });
    }

    [HttpPost("stock-out")]
    public async Task<IActionResult> StockOut(StockOutCommand command)
    {
        var result = await _sender.Send(command);
        return Ok(new { TransactionId = result });
    }
}
