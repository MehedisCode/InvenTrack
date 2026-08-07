namespace InvenTrack.API.Controllers;

using System;
using System.Threading.Tasks;
using InvenTrack.API.Common;
using InvenTrack.Application.Common.Interfaces;
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

    /// <summary>Get all sales (all roles).</summary>
    [HttpGet]
    public async Task<IActionResult> GetAllSales([FromQuery] SaleQueryParameters parameters)
    {
        var sales = await _mediator.Send(new GetAllSalesQuery
        {
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize,
            SortBy = parameters.SortBy,
            SortDescending = parameters.SortDescending,
            SearchTerm = parameters.SearchTerm
        });
        return Ok(sales);
    }

    /// <summary>Get a sale by Id (all roles).</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetSaleById(Guid id)
    {
        var sale = await _mediator.Send(new GetSaleByIdQuery(id));
        if (sale == null)
            return NotFound();

        return Ok(sale);
    }

    /// <summary>Create a new sale (all roles).</summary>
    [HttpPost]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleCommand command)
    {
        var sale = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetSaleById), new { id = sale.Id }, sale);
    }

    /// <summary>Refund a sale (Admin, Manager only).</summary>
    [HttpPost("{id:guid}/refund")]
    [Authorize(Roles = Roles.AdminOrManager)]
    public async Task<IActionResult> RefundSale(Guid id)
    {
        var result = await _mediator.Send(new RefundSaleCommand(id));
        return Ok(new { success = result });
    }
}
