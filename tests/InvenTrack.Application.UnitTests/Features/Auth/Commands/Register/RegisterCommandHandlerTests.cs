namespace InvenTrack.Application.UnitTests.Features.Auth.Commands.Register;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Auth.Commands.Register;
using InvenTrack.Application.Features.Auth.DTOs;
using Moq;
using Xunit;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityService = new();

    private RegisterCommandHandler CreateHandler() => new(_identityService.Object);

    [Fact]
    public async Task Handle_Should_DelegateToIdentityService_WithRoleName()
    {
        var expected = Result<AuthResponse>.Success(new AuthResponse { Token = "jwt-token" });
        _identityService
            .Setup(s => s.RegisterAsync("John Doe", "john@example.com", "password", "Staff", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = CreateHandler();

        var result = await handler.Handle(new RegisterCommand("John Doe", "john@example.com", "password", "Staff"), CancellationToken.None);

        result.Should().Be(expected);
        _identityService.Verify(s => s.RegisterAsync("John Doe", "john@example.com", "password", "Staff", It.IsAny<CancellationToken>()), Times.Once);
    }
}
