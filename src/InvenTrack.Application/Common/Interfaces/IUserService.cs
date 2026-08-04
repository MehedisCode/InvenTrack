namespace InvenTrack.Application.Common.Interfaces;

using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Auth.DTOs;

public interface IUserService
{
    Task<IReadOnlyList<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    Task<Result<UserDto>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<UserDto>> CreateUserAsync(string fullName, string email, string password, string roleName, CancellationToken cancellationToken = default);
    Task<Result<UserDto>> UpdateUserAsync(Guid userId, string fullName, string email, string roleName, bool isActive, CancellationToken cancellationToken = default);
    Task<Result> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
