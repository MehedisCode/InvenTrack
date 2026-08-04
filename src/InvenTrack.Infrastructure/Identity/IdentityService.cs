namespace InvenTrack.Infrastructure.Identity;

using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Auth.DTOs;
using InvenTrack.Domain.Entities;
using Microsoft.AspNetCore.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public IdentityService(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return Result<AuthResponse>.Failure("A user with this email address already exists.");
        }

        var roleName = string.IsNullOrWhiteSpace(request.RoleName) ? "Staff" : request.RoleName;
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role == null)
        {
            role = new Role { Name = roleName };
            var createRoleResult = await _roleManager.CreateAsync(role);
            if (!createRoleResult.Succeeded)
            {
                return Result<AuthResponse>.Failure(createRoleResult.Errors.Select(e => e.Description));
            }
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Email = request.Email,
            UserName = request.Email,
            RoleId = role.Id,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var createUserResult = await _userManager.CreateAsync(user, request.Password);
        if (!createUserResult.Succeeded)
        {
            return Result<AuthResponse>.Failure(createUserResult.Errors.Select(e => e.Description));
        }

        await _userManager.AddToRoleAsync(user, roleName);

        var (token, expiration) = _jwtTokenGenerator.GenerateToken(user, roleName);

        return Result<AuthResponse>.Success(new AuthResponse
        {
            Token = token,
            Expiration = expiration,
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            Role = roleName
        });
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !user.IsActive)
        {
            return Result<AuthResponse>.Failure("Invalid credentials or account is deactivated.");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            return Result<AuthResponse>.Failure("Invalid credentials.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var primaryRole = roles.FirstOrDefault() ?? "Staff";

        var (token, expiration) = _jwtTokenGenerator.GenerateToken(user, primaryRole);

        return Result<AuthResponse>.Success(new AuthResponse
        {
            Token = token,
            Expiration = expiration,
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            Role = primaryRole
        });
    }

    public async Task<Result<UserDto>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Result<UserDto>.Failure("User not found.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var primaryRole = roles.FirstOrDefault() ?? "Staff";

        return Result<UserDto>.Success(new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            Role = primaryRole,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        });
    }
}
