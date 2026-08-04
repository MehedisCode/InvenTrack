namespace InvenTrack.API.Controllers;

using System;
using System.Threading.Tasks;
using InvenTrack.Application.Features.Categories.Commands.CreateCategory;
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

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _sender.Send(new GetAllCategoriesQuery());
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryCommand command)
    {
        var result = await _sender.Send(command);
        return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
    }
}
