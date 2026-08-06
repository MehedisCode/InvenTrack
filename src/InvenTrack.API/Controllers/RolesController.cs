namespace InvenTrack.API.Controllers;

using System.Threading;
using System.Threading.Tasks;
using InvenTrack.API.Common;
using InvenTrack.Application.Features.Roles.Queries.GetAllRoles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>Provides read access to the system's available roles.</summary>
/// <remarks>All endpoints in this controller require the <b>Admin</b> role.</remarks>
[Authorize(Roles = Roles.Admin)]
[ApiController]
[Route("api/[controller]")]
[Tags("Roles")]
[Produces("application/json")]
public class RolesController : ControllerBase
{
    private readonly ISender _sender;

    public RolesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Get all available system roles.</summary>
    /// <remarks>
    /// Returns the list of roles that can be assigned to users.
    /// The system ships with three seeded roles: **Admin**, **Manager**, and **Staff**.
    ///
    /// **Requires:** Admin role.
    ///
    /// Use this endpoint to populate a role picker when creating or updating users.
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the list of roles (id + name).</response>
    /// <response code="401">No valid Bearer token provided.</response>
    /// <response code="403">Caller does not have the Admin role.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllRoles(CancellationToken cancellationToken)
    {
        var roles = await _sender.Send(new GetAllRolesQuery(), cancellationToken);
        return Ok(roles);
    }
}
