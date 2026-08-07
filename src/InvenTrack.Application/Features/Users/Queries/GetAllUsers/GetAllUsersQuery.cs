namespace InvenTrack.Application.Features.Users.Queries.GetAllUsers;

using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Auth.DTOs;
using MediatR;

public class GetAllUsersQuery : UserQueryParameters, IRequest<PaginatedList<UserDto>>
{
}
