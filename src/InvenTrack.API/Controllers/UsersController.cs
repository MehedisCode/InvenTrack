namespace InvenTrack.API.Controllers;

using System;
using System.Threading;
using System.Threading.Tasks;
using InvenTrack.API.Common;
using InvenTrack.Application.Features.Users.Commands.CreateUser;
using InvenTrack.Application.Features.Users.Commands.DeleteUser;
using InvenTrack.Application.Features.Users.Commands.UpdateUser;
using InvenTrack.Application.Features.Users.Queries.GetAllUsers;
using InvenTrack.Application.Features.Users.Queries.GetUserById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>Manages system users — create, read, update, and deactivate accounts.</summary>
/// <remarks>All endpoints in this controller require the <b>Admin</b> role.</remarks>
[Authorize(Roles = Roles.Admin)]
[ApiController]
[Route("api/[controller]")]
[Tags("User Management")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Get a list of all system users.</summary>
    /// <remarks>
    /// Returns every registered user in the system with their role and active status.
    ///
    /// **Requires:** Admin role.
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the list of users.</response>
    /// <response code="401">No valid Bearer token provided.</response>
    /// <response code="403">Caller does not have the Admin role.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        var users = await _sender.Send(new GetAllUsersQuery(), cancellationToken);
        return Ok(users);
    }

    /// <summary>Get a specific user by their Id.</summary>
    /// <remarks>
    /// Returns the full profile of a single user.
    ///
    /// **Requires:** Admin role.
    /// </remarks>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the user profile.</response>
    /// <response code="401">No valid Bearer token provided.</response>
    /// <response code="403">Caller does not have the Admin role.</response>
    /// <response code="404">No user found with the given Id.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetUserByIdQuery(id), cancellationToken);
        if (!result.Succeeded)
        {
            return NotFound(new { errors = result.Errors });
        }

        return Ok(result.Data);
    }

    /// <summary>Create a new user and assign a role.</summary>
    /// <remarks>
    /// Creates a user account with the provided credentials and assigns the named role.
    /// Valid role names: **Admin**, **Manager**, **Staff**.
    ///
    /// **Requires:** Admin role.
    ///
    /// Example request:
    ///
    ///     POST /api/users
    ///     {
    ///       "fullName": "Jane Smith",
    ///       "email": "jane@example.com",
    ///       "password": "Secret123",
    ///       "roleName": "Manager"
    ///     }
    /// </remarks>
    /// <param name="command">New user details including name, email, password, and role.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="201">User created successfully. Returns the new user profile.</response>
    /// <response code="400">Validation failed, email already taken, or role name is invalid.</response>
    /// <response code="401">No valid Bearer token provided.</response>
    /// <response code="403">Caller does not have the Admin role.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command, CancellationToken cancellationToken)
    {
        var result  = await _sender.Send(command, cancellationToken);
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return CreatedAtAction(nameof(GetUserById), new { id = result.Data!.Id }, result.Data);
    }

    /// <summary>Update a user's details, role, or active status.</summary>
    /// <remarks>
    /// Updates an existing user's full name, email, assigned role, and/or active flag.
    /// Setting `isActive` to `false` effectively deactivates the account without deleting it.
    ///
    /// **Requires:** Admin role.
    ///
    /// Example request:
    ///
    ///     PUT /api/users/{id}
    ///     {
    ///       "fullName": "Jane Smith",
    ///       "email": "jane@example.com",
    ///       "roleName": "Staff",
    ///       "isActive": true
    ///     }
    /// </remarks>
    /// <param name="id">The unique identifier of the user to update.</param>
    /// <param name="command">Updated user details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">User updated successfully. Returns the updated profile.</response>
    /// <response code="400">Validation failed or role name is invalid.</response>
    /// <response code="401">No valid Bearer token provided.</response>
    /// <response code="403">Caller does not have the Admin role.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserCommand command, CancellationToken cancellationToken)
    {
        // Ensure the ID in the route matches the ID in the body
        if (id != command.Id)
        {
            return BadRequest(new { errors = new[] { "Route ID and Body ID mismatch." } });
        }
        var result  = await _sender.Send(command, cancellationToken);
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(result.Data);
    }

    /// <summary>Delete a user by their Id.</summary>
    /// <remarks>
    /// Permanently removes the user account from the system.
    ///
    /// **Requires:** Admin role.
    /// </remarks>
    /// <param name="id">The unique identifier of the user to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">User deleted successfully.</response>
    /// <response code="400">Delete operation failed (e.g. user has associated records).</response>
    /// <response code="401">No valid Bearer token provided.</response>
    /// <response code="403">Caller does not have the Admin role.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand(id);
        var result  = await _sender.Send(command, cancellationToken);
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return NoContent();
    }
}
