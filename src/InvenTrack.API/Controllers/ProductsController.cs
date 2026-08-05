namespace InvenTrack.API.Controllers;

using System;
using System.Threading.Tasks;
using InvenTrack.API.Common;
using InvenTrack.Application.Features.Products.Commands.CreateProduct;
using InvenTrack.Application.Features.Products.Commands.DeleteProduct;
using InvenTrack.Application.Features.Products.Commands.UpdateProduct;
using InvenTrack.Application.Features.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly ISender _sender;

    public ProductsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Get all products (all roles).</summary>
    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] GetProductsQuery query)
    {
        var result = await _sender.Send(query);
        return Ok(result);
    }

    /// <summary>Create a new product (Admin, Manager only).</summary>
    [HttpPost]
    [Authorize(Roles = Roles.AdminOrManager)]
    public async Task<IActionResult> Create(CreateProductCommand command)
    {
        var result = await _sender.Send(command);
        return CreatedAtAction(nameof(GetProducts), new { id = result.Id }, result);
    }

    /// <summary>Update an existing product (Admin, Manager only).</summary>
    [HttpPut("{id}")]
    [Authorize(Roles = Roles.AdminOrManager)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request)
    {
        var result = await _sender.Send(new UpdateProductCommand(
            id, request.Name, request.SKU, request.Description,
            request.UnitPrice, request.CostPrice, request.CategoryId, request.IsActive));
        return Ok(result);
    }

    /// <summary>Soft-delete a product (Admin, Manager only).</summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.AdminOrManager)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _sender.Send(new DeleteProductCommand(id));
        return NoContent();
    }
}

/// <summary>Request body for updating a product (Id comes from route).</summary>
public record UpdateProductRequest(
    string  Name,
    string  SKU,
    string? Description,
    decimal UnitPrice,
    decimal CostPrice,
    Guid    CategoryId,
    bool    IsActive);
