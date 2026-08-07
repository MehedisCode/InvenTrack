namespace InvenTrack.Application.UnitTests.Features.Users.Queries.GetAllUsers;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Auth.DTOs;
using InvenTrack.Application.Features.Users.Queries.GetAllUsers;
using Moq;
using Xunit;

public class GetAllUsersQueryHandlerTests
{
    private readonly Mock<IUserService> _userService = new();

    private GetAllUsersQueryHandler CreateHandler() => new(_userService.Object);

    [Fact]
    public async Task Handle_Should_ReturnAllUsers()
    {
        var users = new List<UserDto> { new() { Email = "a@b.com" }, new() { Email = "c@d.com" } };
        _userService
            .Setup(s => s.GetAllUsersAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        var handler = CreateHandler();

        var result = await handler.Handle(new GetAllUsersQuery(), CancellationToken.None);

        result.Should().BeEquivalentTo(users);
        _userService.Verify(s => s.GetAllUsersAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
