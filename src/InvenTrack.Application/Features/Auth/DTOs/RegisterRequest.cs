namespace InvenTrack.Application.Features.Auth.DTOs;

using InvenTrack.Application.Common.Models;
using MediatR;

public class RegisterRequest : IRequest<Result<AuthResponse>>
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string RoleName { get; set; } = "Staff";
}
