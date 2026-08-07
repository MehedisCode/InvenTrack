namespace InvenTrack.Application.UnitTests.Features.Auth.Validators;

using FluentAssertions;
using InvenTrack.Application.Features.Auth.Commands.Register;
using InvenTrack.Application.Features.Auth.Validators;
using Xunit;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenCommandIsValid()
    {
        var command = new RegisterCommand("John Doe", "john@example.com", "password123", "Staff");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_ShouldFail_WhenFullNameIsMissing(string? fullName)
    {
        var command = new RegisterCommand(fullName!, "john@example.com", "password123", "Staff");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FullName");
    }

    [Fact]
    public void Validate_ShouldFail_WhenFullNameExceeds100Characters()
    {
        var longName = new string('A', 101);
        var command = new RegisterCommand(longName, "john@example.com", "password123", "Staff");

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
        var command = new RegisterCommand("John Doe", email!, "password123", "Staff");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_ShouldFail_WhenPasswordIsMissing(string? password)
    {
        var command = new RegisterCommand("John Doe", "john@example.com", password!, "Staff");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public void Validate_ShouldFail_WhenPasswordIsShorterThan6Characters()
    {
        var command = new RegisterCommand("John Doe", "john@example.com", "12345", "Staff");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_ShouldFail_WhenRoleNameIsMissing(string? roleName)
    {
        var command = new RegisterCommand("John Doe", "john@example.com", "password123", roleName!);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "RoleName");
    }
}
