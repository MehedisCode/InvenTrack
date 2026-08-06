namespace InvenTrack.Application.Common.Interfaces;

using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Auth.DTOs;

public interface IIdentityService
{
    Task<Result<AuthResponse>> RegisterAsync(string fullName, string email, string password, string roleName, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<Result<UserDto>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
