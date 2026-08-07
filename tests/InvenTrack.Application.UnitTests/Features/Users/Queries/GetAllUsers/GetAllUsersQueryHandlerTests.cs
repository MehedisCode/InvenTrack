namespace InvenTrack.Application.UnitTests.Features.Users.Queries.GetAllUsers;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
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
            .Setup(s => s.GetUsersPaginatedAsync(It.IsAny<UserQueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedList<UserDto>(users, users.Count, 1, 10));

        var handler = CreateHandler();

        var result = await handler.Handle(new GetAllUsersQuery(), CancellationToken.None);

        result.Items.Should().BeEquivalentTo(users);
        result.TotalCount.Should().Be(2);
        _userService.Verify(s => s.GetUsersPaginatedAsync(It.IsAny<UserQueryParameters>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
