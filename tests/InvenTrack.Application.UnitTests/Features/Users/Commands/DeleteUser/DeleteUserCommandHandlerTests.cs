namespace InvenTrack.Application.UnitTests.Features.Users.Commands.DeleteUser;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Users.Commands.DeleteUser;
using Moq;
using Xunit;

public class DeleteUserCommandHandlerTests
{
    private readonly Mock<IUserService> _userService = new();

    private DeleteUserCommandHandler CreateHandler() => new(_userService.Object);

    [Fact]
    public async Task Handle_Should_DelegateToUserService()
    {
        var userId = Guid.NewGuid();
        var expected = Result.Success();
        _userService
            .Setup(s => s.DeleteUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = CreateHandler();

        var result = await handler.Handle(new DeleteUserCommand(userId), CancellationToken.None);

        result.Should().Be(expected);
        _userService.Verify(s => s.DeleteUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
