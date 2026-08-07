namespace InvenTrack.Application.UnitTests.Features.Users.Commands.UpdateUser;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Auth.DTOs;
using InvenTrack.Application.Features.Users.Commands.UpdateUser;
using Moq;
using Xunit;

public class UpdateUserCommandHandlerTests
{
    private readonly Mock<IUserService> _userService = new();

    private UpdateUserCommandHandler CreateHandler() => new(_userService.Object);

    [Fact]
    public async Task Handle_Should_DelegateToUserService()
    {
        var userId = Guid.NewGuid();
        var expected = Result<UserDto>.Success(new UserDto { Id = userId });
        _userService
            .Setup(s => s.UpdateUserAsync(userId, "John", "john@example.com", "Manager", true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = CreateHandler();

        var result = await handler.Handle(new UpdateUserCommand(userId, "John", "john@example.com", "Manager", true), CancellationToken.None);

        result.Should().Be(expected);
        _userService.Verify(s => s.UpdateUserAsync(userId, "John", "john@example.com", "Manager", true, It.IsAny<CancellationToken>()), Times.Once);
    }
}
