namespace InvenTrack.API.Controllers;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvenTrack.API.Common;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Suppliers.Commands.AssignProducts;
using InvenTrack.Application.Features.Suppliers.Commands.CreateSupplier;
using InvenTrack.Application.Features.Suppliers.Commands.DeleteSupplier;
using InvenTrack.Application.Features.Suppliers.Commands.UpdateSupplier;
using InvenTrack.Application.Features.Suppliers.Queries.GetAllSuppliers;
using InvenTrack.Application.Features.Suppliers.Queries.GetSupplierById;
using InvenTrack.Application.Features.Suppliers.Queries.SearchSuppliers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SuppliersController : ControllerBase
{
    private readonly ISender _sender;

    public SuppliersController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Get all suppliers (all roles).</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] SupplierQueryParameters parameters)
    {
        var result = await _sender.Send(new GetAllSuppliersQuery
        {
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize,
            SortBy = parameters.SortBy,
            SortDescending = parameters.SortDescending,
            SearchTerm = parameters.SearchTerm
        });
        return Ok(result);
    }

    /// <summary>Search suppliers (all roles).</summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return BadRequest("Search term cannot be empty.");

        var result = await _sender.Send(new SearchSuppliersQuery(term));
        return Ok(result);
    }

    /// <summary>Get a supplier by Id (all roles).</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _sender.Send(new GetSupplierByIdQuery(id));
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>Create a new supplier (Admin, Manager only).</summary>
    [HttpPost]
    [Authorize(Roles = Roles.AdminOrManager)]
    public async Task<IActionResult> Create(CreateSupplierCommand command)
    {
        var result = await _sender.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Update a supplier (Admin, Manager only).</summary>
    [HttpPut("{id}")]
    [Authorize(Roles = Roles.AdminOrManager)]
    public async Task<IActionResult> Update(Guid id, UpdateSupplierCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID in URL does not match ID in command.");

        await _sender.Send(command);
        return NoContent();
    }

    /// <summary>Delete a supplier (Admin, Manager only).</summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.AdminOrManager)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _sender.Send(new DeleteSupplierCommand(id));
        return NoContent();
    }

    /// <summary>Assign products to a supplier (Admin, Manager only).</summary>
    [HttpPost("{id}/products")]
    [Authorize(Roles = Roles.AdminOrManager)]
    public async Task<IActionResult> AssignProducts(Guid id, [FromBody] List<Guid> productIds)
    {
        if (productIds == null || productIds.Count == 0)
            return BadRequest("At least one product ID must be provided.");

        await _sender.Send(new AssignProductsToSupplierCommand(id, productIds));
        return Ok(new { message = "Products assigned successfully." });
    }
}
