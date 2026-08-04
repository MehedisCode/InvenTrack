namespace InvenTrack.Application.Features.Auth.DTOs;

public class RegisterRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string RoleName { get; set; } = "Staff";
}
