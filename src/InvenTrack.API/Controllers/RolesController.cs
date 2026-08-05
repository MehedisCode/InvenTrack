namespace InvenTrack.API.Controllers;

using InvenTrack.API.Common;
using InvenTrack.Application.Features.Roles.Queries.GetAllRoles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = Roles.Admin)]
[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly ISender _sender;

    public RolesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRoles(CancellationToken cancellationToken)
    {
        var roles = await _sender.Send(new GetAllRolesQuery(), cancellationToken);
        return Ok(roles);
    }
}
