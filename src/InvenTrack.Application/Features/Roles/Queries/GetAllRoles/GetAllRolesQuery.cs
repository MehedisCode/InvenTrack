namespace InvenTrack.Application.Features.Roles.Queries.GetAllRoles;

using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Roles.DTOs;
using MediatR;

public record GetAllRolesQuery : IRequest<IReadOnlyList<RoleDto>>;

public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, IReadOnlyList<RoleDto>>
{
    private readonly IRoleService _roleService;

    public GetAllRolesQueryHandler(IRoleService roleService)
    {
        _roleService = roleService;
    }

    public async Task<IReadOnlyList<RoleDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        return await _roleService.GetAllRolesAsync(cancellationToken);
    }
}
