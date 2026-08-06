namespace InvenTrack.API.Controllers;

using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Auth.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>Handles user authentication — register, login, and current-user profile.</summary>
[ApiController]
[Route("api/[controller]")]
[Tags("Authentication")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<LoginRequest> _loginValidator;

    public AuthController(
        IIdentityService identityService,
        ICurrentUserService currentUserService,
        IValidator<RegisterRequest> registerValidator,
        IValidator<LoginRequest> loginValidator)
    {
        _identityService    = identityService;
        _currentUserService = currentUserService;
        _registerValidator  = registerValidator;
        _loginValidator     = loginValidator;
    }

    /// <summary>Register a new user account.</summary>
    /// <remarks>
    /// Creates a new user with the given credentials and assigns the specified role.
    /// Returns a JWT token on success.
    ///
    /// **No authentication required.**
    ///
    /// Example request:
    ///
    ///     POST /api/auth/register
    ///     {
    ///       "fullName": "John Doe",
    ///       "email": "john@example.com",
    ///       "password": "Secret123",
    ///       "roleName": "Staff"
    ///     }
    /// </remarks>
    /// <param name="request">Registration details including full name, email, password, and role.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Registration successful. Returns a JWT access token.</response>
    /// <response code="400">Validation failed or email already in use.</response>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _registerValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        var result = await _identityService.RegisterAsync(request, cancellationToken);
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(result.Data);
    }

    /// <summary>Authenticate and receive a JWT token.</summary>
    /// <remarks>
    /// Validates credentials against the identity store. On success, returns a signed
    /// JWT token to be used as a Bearer token in subsequent requests.
    ///
    /// **No authentication required.**
    ///
    /// Example request:
    ///
    ///     POST /api/auth/login
    ///     {
    ///       "email": "john@example.com",
    ///       "password": "Secret123"
    ///     }
    /// </remarks>
    /// <param name="request">Login credentials (email + password).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Login successful. Returns a JWT access token.</response>
    /// <response code="400">Validation failed (missing or malformed fields).</response>
    /// <response code="401">Invalid email or password.</response>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _loginValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        var result = await _identityService.LoginAsync(request, cancellationToken);
        if (!result.Succeeded)
        {
            return Unauthorized(new { errors = result.Errors });
        }

        return Ok(result.Data);
    }

    /// <summary>Get the currently authenticated user's profile.</summary>
    /// <remarks>
    /// Decodes the Bearer token and returns the full profile of the authenticated user.
    ///
    /// **Requires:** Any authenticated user (any role).
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the current user's profile.</response>
    /// <response code="401">No valid Bearer token provided.</response>
    /// <response code="404">User referenced in token no longer exists.</response>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Unauthorized(new { error = "User identity could not be retrieved from the token." });
        }

        var result = await _identityService.GetUserByIdAsync(userId.Value, cancellationToken);
        if (!result.Succeeded)
        {
            return NotFound(new { errors = result.Errors });
        }

        return Ok(result.Data);
    }
}
