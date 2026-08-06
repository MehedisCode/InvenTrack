namespace InvenTrack.Application.Features.Users.Queries.GetAllUsers;

using InvenTrack.Application.Features.Auth.DTOs;
using MediatR;

public record GetAllUsersQuery : IRequest<IReadOnlyList<UserDto>>;
