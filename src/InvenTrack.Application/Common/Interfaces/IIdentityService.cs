namespace InvenTrack.Application.Common.Interfaces;

using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Auth.DTOs;

public interface IIdentityService
{
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<Result<UserDto>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
