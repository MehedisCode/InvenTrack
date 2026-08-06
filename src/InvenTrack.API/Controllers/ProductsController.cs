namespace InvenTrack.API.Controllers;

using System;
using System.Threading.Tasks;
using InvenTrack.API.Common;
using InvenTrack.Application.Features.Products.Commands.CreateProduct;
using InvenTrack.Application.Features.Products.Commands.DeleteProduct;
using InvenTrack.Application.Features.Products.Commands.UpdateProduct;
using InvenTrack.Application.Features.Products.DTOs;
using InvenTrack.Application.Features.Products.Queries.GetProductById;
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts([FromQuery] GetProductsQuery query)
    {
        var result = await _sender.Send(query);
        return Ok(result);
    }

    /// <summary>Get a product by Id (all roles).</summary>
    /// <param name="id">The GUID of the product to retrieve.</param>
    /// <response code="200">Returns the product.</response>
    /// <response code="404">If the product is not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        var result = await _sender.Send(new GetProductByIdQuery(id));
        return Ok(result);
    }

    /// <summary>Create a new product (Admin, Manager only).</summary>
    [HttpPost]
    [Authorize(Roles = Roles.AdminOrManager)]
    public async Task<IActionResult> Create(CreateProductCommand command)
    {
        var result = await _sender.Send(command);
        return CreatedAtAction(nameof(GetProductById), new { id = result.Id }, result);
    }

    /// <summary>Update an existing product (Admin, Manager only).</summary>
    [HttpPut("{id}")]
    [Authorize(Roles = Roles.AdminOrManager)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request)
    {
        var result = await _sender.Send(new UpdateProductCommand(
            id, request.Name, request.SKU, request.Description,
            request.PurchasePrice, request.SellingPrice, request.CategoryId, request.IsActive));
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
