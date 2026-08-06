namespace InvenTrack.Application.Features.Roles.Queries.GetAllRoles;

using InvenTrack.Application.Features.Roles.DTOs;
using MediatR;

public record GetAllRolesQuery : IRequest<IReadOnlyList<RoleDto>>;
