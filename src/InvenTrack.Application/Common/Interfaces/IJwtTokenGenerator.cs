namespace InvenTrack.Application.Common.Interfaces;

using InvenTrack.Domain.Entities;

public interface IJwtTokenGenerator
{
    (string Token, DateTime Expiration) GenerateToken(User user, string roleName);
}
