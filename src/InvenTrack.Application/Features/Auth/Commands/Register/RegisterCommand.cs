namespace InvenTrack.Application.Features.Auth.Commands.Register;

using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Auth.DTOs;
using MediatR;

public record RegisterCommand(
    string FullName,
    string Email,
    string Password,
    string RoleName = "Staff") : IRequest<Result<AuthResponse>>;
