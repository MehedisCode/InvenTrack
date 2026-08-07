namespace InvenTrack.Application.UnitTests.Features.Users.Commands.UpdateUser;

using FluentAssertions;
using InvenTrack.Application.Features.Users.Commands.UpdateUser;
using Xunit;

public class UpdateUserCommandValidatorTests
{
    private readonly UpdateUserCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenCommandIsValid()
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), "John Doe", "john@example.com", "Staff", true);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldFail_WhenIdIsEmpty()
    {
        var command = new UpdateUserCommand(Guid.Empty, "John Doe", "john@example.com", "Staff", true);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_ShouldFail_WhenFullNameIsMissing(string? fullName)
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), fullName!, "john@example.com", "Staff", true);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FullName");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("not-an-email")]
    public void Validate_ShouldFail_WhenEmailIsInvalid(string? email)
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), "John Doe", email!, "Staff", true);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_ShouldFail_WhenRoleNameIsMissing(string? roleName)
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), "John Doe", "john@example.com", roleName!, true);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "RoleName");
    }
}
