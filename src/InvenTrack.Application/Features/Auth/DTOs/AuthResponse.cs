namespace InvenTrack.Application.Features.Auth.DTOs;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
