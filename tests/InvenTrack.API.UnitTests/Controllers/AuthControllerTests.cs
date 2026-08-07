namespace InvenTrack.API.UnitTests;

using FluentAssertions;
using InvenTrack.API.Controllers;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Auth.Commands.Login;
using InvenTrack.Application.Features.Auth.Commands.Register;
using InvenTrack.Application.Features.Auth.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class AuthControllerTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly Mock<IIdentityService> _identityService = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();

    private AuthController CreateController() => new(_mediator.Object, _identityService.Object, _currentUserService.Object);

    [Fact]
    public async Task Register_ShouldReturnOk_WhenSucceeded()
    {
        var dto = new AuthResponse { Token = "token" };
        _mediator
            .Setup(m => m.Send(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AuthResponse>.Success(dto));

        var controller = CreateController();

        var result = await controller.Register(new RegisterCommand("John", "john@example.com", "password", "Staff"), CancellationToken.None);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(dto);
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenFailed()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AuthResponse>.Failure("Email already in use."));

        var controller = CreateController();

        var result = await controller.Register(new RegisterCommand("John", "john@example.com", "password", "Staff"), CancellationToken.None);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenSucceeded()
    {
        var dto = new AuthResponse { Token = "token" };
        _mediator
            .Setup(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AuthResponse>.Success(dto));

        var controller = CreateController();

        var result = await controller.Login(new LoginCommand("john@example.com", "password"), CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenFailed()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AuthResponse>.Failure("Invalid credentials."));

        var controller = CreateController();

        var result = await controller.Login(new LoginCommand("john@example.com", "wrong"), CancellationToken.None);

        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task GetCurrentUser_ShouldReturnUnauthorized_WhenUserIdIsNull()
    {
        _currentUserService.SetupGet(s => s.UserId).Returns((Guid?)null);

        var controller = CreateController();

        var result = await controller.GetCurrentUser(CancellationToken.None);

        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task GetCurrentUser_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        var userId = Guid.NewGuid();
        _currentUserService.SetupGet(s => s.UserId).Returns(userId);
        _identityService
            .Setup(s => s.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<UserDto>.Failure("User not found."));

        var controller = CreateController();

        var result = await controller.GetCurrentUser(CancellationToken.None);

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetCurrentUser_ShouldReturnOk_WhenUserExists()
    {
        var userId = Guid.NewGuid();
        var dto = new UserDto { Id = userId, FullName = "John" };
        _currentUserService.SetupGet(s => s.UserId).Returns(userId);
        _identityService
            .Setup(s => s.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<UserDto>.Success(dto));

        var controller = CreateController();

        var result = await controller.GetCurrentUser(CancellationToken.None);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(dto);
    }
}
