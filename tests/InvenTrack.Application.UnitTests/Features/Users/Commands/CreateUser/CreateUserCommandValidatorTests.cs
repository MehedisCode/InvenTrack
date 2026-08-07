namespace InvenTrack.Application.UnitTests.Features.Users.Commands.CreateUser;

using FluentAssertions;
using InvenTrack.Application.Features.Users.Commands.CreateUser;
using Xunit;

public class CreateUserCommandValidatorTests
{
    private readonly CreateUserCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenCommandIsValid()
    {
        var command = new CreateUserCommand("John Doe", "john@example.com", "password123", "Staff");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_ShouldFail_WhenFullNameIsMissing(string? fullName)
    {
        var command = new CreateUserCommand(fullName!, "john@example.com", "password123", "Staff");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FullName");
    }

    [Fact]
    public void Validate_ShouldFail_WhenFullNameExceeds100Characters()
    {
        var longName = new string('A', 101);
        var command = new CreateUserCommand(longName, "john@example.com", "password123", "Staff");

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
        var command = new CreateUserCommand("John Doe", email!, "password123", "Staff");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_ShouldFail_WhenPasswordIsMissing(string? password)
    {
        var command = new CreateUserCommand("John Doe", "john@example.com", password!, "Staff");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public void Validate_ShouldFail_WhenPasswordIsShorterThan6Characters()
    {
        var command = new CreateUserCommand("John Doe", "john@example.com", "12345", "Staff");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_ShouldFail_WhenRoleNameIsMissing(string? roleName)
    {
        var command = new CreateUserCommand("John Doe", "john@example.com", "password123", roleName!);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "RoleName");
    }
}
