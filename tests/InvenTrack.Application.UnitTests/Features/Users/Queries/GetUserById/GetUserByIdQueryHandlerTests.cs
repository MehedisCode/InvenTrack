namespace InvenTrack.Application.UnitTests.Features.Users.Queries.GetUserById;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Auth.DTOs;
using InvenTrack.Application.Features.Users.Queries.GetUserById;
using Moq;
using Xunit;

public class GetUserByIdQueryHandlerTests
{
    private readonly Mock<IUserService> _userService = new();

    private GetUserByIdQueryHandler CreateHandler() => new(_userService.Object);

    [Fact]
    public async Task Handle_Should_DelegateToUserService()
    {
        var userId = Guid.NewGuid();
        var expected = Result<UserDto>.Success(new UserDto { Id = userId });
        _userService
            .Setup(s => s.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = CreateHandler();

        var result = await handler.Handle(new GetUserByIdQuery(userId), CancellationToken.None);

        result.Should().Be(expected);
        _userService.Verify(s => s.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
