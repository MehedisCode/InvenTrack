namespace InvenTrack.Application.Features.Users.Commands.CreateUser;

using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Auth.DTOs;
using MediatR;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IUserService _userService;

    public CreateUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        return await _userService.CreateUserAsync(request.FullName, request.Email, request.Password, request.RoleName, cancellationToken);
    }
}
