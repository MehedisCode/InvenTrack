namespace InvenTrack.Application.Features.Users.Commands.DeleteUser;

using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using MediatR;

public record DeleteUserCommand(Guid Id) : IRequest<Result>;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
{
    private readonly IUserService _userService;

    public DeleteUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        return await _userService.DeleteUserAsync(request.Id, cancellationToken);
    }
}
