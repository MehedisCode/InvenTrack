namespace InvenTrack.Application.UnitTests.Features.Roles.Queries.GetAllRoles;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Roles.DTOs;
using InvenTrack.Application.Features.Roles.Queries.GetAllRoles;
using Moq;
using Xunit;

public class GetAllRolesQueryHandlerTests
{
    private readonly Mock<IRoleService> _roleService = new();

    private GetAllRolesQueryHandler CreateHandler() => new(_roleService.Object);

    [Fact]
    public async Task Handle_Should_ReturnAllRoles()
    {
        var roles = new List<RoleDto> { new() { Name = "Admin" }, new() { Name = "Staff" } };
        _roleService
            .Setup(s => s.GetAllRolesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(roles);

        var handler = CreateHandler();

        var result = await handler.Handle(new GetAllRolesQuery(), CancellationToken.None);

        result.Should().BeEquivalentTo(roles);
        _roleService.Verify(s => s.GetAllRolesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
