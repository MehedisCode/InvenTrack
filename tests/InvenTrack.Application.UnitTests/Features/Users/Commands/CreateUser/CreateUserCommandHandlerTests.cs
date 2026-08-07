namespace InvenTrack.Application.UnitTests.Features.Users.Commands.CreateUser;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Auth.DTOs;
using InvenTrack.Application.Features.Users.Commands.CreateUser;
using Moq;
using Xunit;

public class CreateUserCommandHandlerTests
{
    private readonly Mock<IUserService> _userService = new();

    private CreateUserCommandHandler CreateHandler() => new(_userService.Object);

    [Fact]
    public async Task Handle_Should_DelegateToUserService()
    {
        var expected = Result<UserDto>.Success(new UserDto { Email = "john@example.com" });
        _userService
            .Setup(s => s.CreateUserAsync("John Doe", "john@example.com", "password", "Staff", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = CreateHandler();

        var result = await handler.Handle(new CreateUserCommand("John Doe", "john@example.com", "password", "Staff"), CancellationToken.None);

        result.Should().Be(expected);
        _userService.Verify(s => s.CreateUserAsync("John Doe", "john@example.com", "password", "Staff", It.IsAny<CancellationToken>()), Times.Once);
    }
}
