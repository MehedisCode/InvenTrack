namespace InvenTrack.API.Controllers;

using System;
using System.Threading.Tasks;
using InvenTrack.API.Common;
using InvenTrack.Application.Features.Categories.Commands.CreateCategory;
using InvenTrack.Application.Features.Categories.Commands.DeleteCategory;
using InvenTrack.Application.Features.Categories.Commands.UpdateCategory;
using InvenTrack.Application.Features.Categories.Queries.GetAllCategories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly ISender _sender;

    public CategoriesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Get all categories (all roles).</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _sender.Send(new GetAllCategoriesQuery());
        return Ok(result);
    }

    /// <summary>Create a new category (Admin, Manager only).</summary>
    [HttpPost]
    [Authorize(Roles = Roles.AdminOrManager)]
    public async Task<IActionResult> Create(CreateCategoryCommand command)
    {
        var result = await _sender.Send(command);
        return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
    }

    /// <summary>Update an existing category (Admin, Manager only).</summary>
    [HttpPut("{id}")]
    [Authorize(Roles = Roles.AdminOrManager)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest request)
    {
        var result = await _sender.Send(new UpdateCategoryCommand(id, request.Name, request.Description));
        return Ok(result);
    }

    /// <summary>Delete a category (Admin, Manager only).</summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.AdminOrManager)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _sender.Send(new DeleteCategoryCommand(id));
        return NoContent();
    }
}

/// <summary>Request body for updating a category (Id comes from route).</summary>
public record UpdateCategoryRequest(string Name, string? Description);
