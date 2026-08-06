namespace InvenTrack.Application.Features.Users.Commands.DeleteUser;

using InvenTrack.Application.Common.Models;
using MediatR;

public record DeleteUserCommand(Guid Id) : IRequest<Result>;
