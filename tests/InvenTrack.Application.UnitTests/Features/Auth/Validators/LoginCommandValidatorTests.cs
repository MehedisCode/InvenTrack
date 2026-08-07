namespace InvenTrack.Application.UnitTests.Features.Auth.Validators;

using FluentAssertions;
using InvenTrack.Application.Features.Auth.Commands.Login;
using InvenTrack.Application.Features.Auth.Validators;
using Xunit;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenCommandIsValid()
    {
        var command = new LoginCommand("john@example.com", "password123");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("not-an-email")]
    public void Validate_ShouldFail_WhenEmailIsInvalid(string? email)
    {
        var command = new LoginCommand(email!, "password123");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_ShouldFail_WhenPasswordIsMissing(string? password)
    {
        var command = new LoginCommand("john@example.com", password!);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }
}
