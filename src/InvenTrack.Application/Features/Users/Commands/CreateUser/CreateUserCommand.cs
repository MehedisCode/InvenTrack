namespace InvenTrack.Application.Features.Users.Commands.CreateUser;

using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Auth.DTOs;
using MediatR;

public record CreateUserCommand(string FullName, string Email, string Password, string RoleName) : IRequest<Result<UserDto>>;
