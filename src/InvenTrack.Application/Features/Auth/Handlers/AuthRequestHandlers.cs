namespace InvenTrack.Application.Features.Auth.Handlers;

using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Auth.DTOs;
using MediatR;

public class RegisterRequestHandler : IRequestHandler<RegisterRequest, Result<AuthResponse>>
{
    private readonly IIdentityService _identityService;

    public RegisterRequestHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        return await _identityService.RegisterAsync(request, cancellationToken);
    }
}

public class LoginRequestHandler : IRequestHandler<LoginRequest, Result<AuthResponse>>
{
    private readonly IIdentityService _identityService;

    public LoginRequestHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<AuthResponse>> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        return await _identityService.LoginAsync(request, cancellationToken);
    }
}
