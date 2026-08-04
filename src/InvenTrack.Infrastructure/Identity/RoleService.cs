namespace InvenTrack.Infrastructure.Identity;

using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Roles.DTOs;
using InvenTrack.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class RoleService : IRoleService
{
    private readonly RoleManager<Role> _roleManager;

    public RoleService(RoleManager<Role> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<IReadOnlyList<RoleDto>> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _roleManager.Roles
            .AsNoTracking()
            .Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name ?? string.Empty
            })
            .ToListAsync(cancellationToken);

        return roles;
    }
}
