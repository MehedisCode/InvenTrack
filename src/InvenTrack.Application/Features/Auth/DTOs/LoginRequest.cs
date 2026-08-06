namespace InvenTrack.Application.Features.Auth.DTOs;

using InvenTrack.Application.Common.Models;
using MediatR;

public class LoginRequest : IRequest<Result<AuthResponse>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
