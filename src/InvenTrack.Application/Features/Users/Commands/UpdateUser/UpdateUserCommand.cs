namespace InvenTrack.Application.Features.Users.Commands.UpdateUser;

using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Auth.DTOs;
using MediatR;

public record UpdateUserCommand(Guid Id, string FullName, string Email, string RoleName, bool IsActive) : IRequest<Result<UserDto>>;
