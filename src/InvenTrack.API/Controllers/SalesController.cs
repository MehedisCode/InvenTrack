namespace InvenTrack.API.Controllers;

using System;
using System.Threading.Tasks;
using InvenTrack.Application.Features.Sales.Commands.CreateSale;
using InvenTrack.Application.Features.Sales.Commands.RefundSale;
using InvenTrack.Application.Features.Sales.Queries.GetAllSales;
using InvenTrack.Application.Features.Sales.Queries.GetSaleById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SalesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSales()
    {
        var sales = await _mediator.Send(new GetAllSalesQuery());
        return Ok(sales);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetSaleById(Guid id)
    {
        var sale = await _mediator.Send(new GetSaleByIdQuery(id));
        if (sale == null)
            return NotFound();

        return Ok(sale);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleCommand command)
    {
        var sale = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetSaleById), new { id = sale.Id }, sale);
    }

    [HttpPost("{id:guid}/refund")]
    public async Task<IActionResult> RefundSale(Guid id)
    {
        var result = await _mediator.Send(new RefundSaleCommand(id));
        return Ok(new { success = result });
    }
}
