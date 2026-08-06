namespace InvenTrack.Application.Features.Auth.Commands.Login;

using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Auth.DTOs;
using MediatR;

public record LoginCommand(string Email, string Password) : IRequest<Result<AuthResponse>>;
