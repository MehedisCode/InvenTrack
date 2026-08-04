namespace InvenTrack.Application.Features.Users.Queries.GetUserById;

using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Auth.DTOs;
using MediatR;

public record GetUserByIdQuery(Guid Id) : IRequest<Result<UserDto>>;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IUserService _userService;

    public GetUserByIdQueryHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        return await _userService.GetUserByIdAsync(request.Id, cancellationToken);
    }
}
