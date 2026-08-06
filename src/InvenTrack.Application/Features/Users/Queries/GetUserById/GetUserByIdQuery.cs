namespace InvenTrack.Application.Features.Users.Queries.GetUserById;

using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Auth.DTOs;
using MediatR;

public record GetUserByIdQuery(Guid Id) : IRequest<Result<UserDto>>;
