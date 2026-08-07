namespace InvenTrack.API.Controllers;

using System;
using System.Threading.Tasks;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Purchases.Commands.CreatePurchase;
using InvenTrack.Application.Features.Purchases.Queries.GetAllPurchases;
using InvenTrack.Application.Features.Purchases.Queries.GetPurchaseById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PurchasesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PurchasesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Get all purchases (all roles).</summary>
    [HttpGet]
    public async Task<IActionResult> GetAllPurchases([FromQuery] PurchaseQueryParameters parameters)
    {
        var purchases = await _mediator.Send(new GetAllPurchasesQuery
        {
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize,
            SortBy = parameters.SortBy,
            SortDescending = parameters.SortDescending,
            SearchTerm = parameters.SearchTerm
        });
        return Ok(purchases);
    }

    /// <summary>Get a purchase by Id (all roles).</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPurchaseById(Guid id)
    {
        var purchase = await _mediator.Send(new GetPurchaseByIdQuery(id));
        if (purchase == null)
            return NotFound();

        return Ok(purchase);
    }

    /// <summary>Create a new purchase (all roles).</summary>
    [HttpPost]
    public async Task<IActionResult> CreatePurchase([FromBody] CreatePurchaseCommand command)
    {
        var purchase = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetPurchaseById), new { id = purchase.Id }, purchase);
    }
}
