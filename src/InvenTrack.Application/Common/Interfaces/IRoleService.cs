namespace InvenTrack.Application.Common.Interfaces;

using InvenTrack.Application.Features.Roles.DTOs;

public interface IRoleService
{
    Task<IReadOnlyList<RoleDto>> GetAllRolesAsync(CancellationToken cancellationToken = default);
}
