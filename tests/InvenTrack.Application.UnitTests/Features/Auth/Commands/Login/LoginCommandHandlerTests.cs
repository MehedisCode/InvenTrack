namespace InvenTrack.Application.UnitTests.Features.Auth.Commands.Login;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Auth.Commands.Login;
using InvenTrack.Application.Features.Auth.DTOs;
using Moq;
using Xunit;

public class LoginCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityService = new();

    private LoginCommandHandler CreateHandler() => new(_identityService.Object);

    [Fact]
    public async Task Handle_Should_DelegateToIdentityService()
    {
        var expected = Result<AuthResponse>.Success(new AuthResponse { Token = "jwt-token" });
        _identityService
            .Setup(s => s.LoginAsync("john@example.com", "password", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = CreateHandler();

        var result = await handler.Handle(new LoginCommand("john@example.com", "password"), CancellationToken.None);

        result.Should().Be(expected);
        _identityService.Verify(s => s.LoginAsync("john@example.com", "password", It.IsAny<CancellationToken>()), Times.Once);
    }
}
