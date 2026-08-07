namespace InvenTrack.Infrastructure.UnitTests.Identity;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using InvenTrack.Domain.Entities;
using InvenTrack.Infrastructure.Identity;
using Microsoft.Extensions.Options;
using Xunit;

public class JwtTokenGeneratorTests
{
    private static readonly JwtSettings Settings = new()
    {
        // Must be at least 128 bits for HmacSha256.
        Secret = "this-is-a-very-long-test-secret-key-32b",
        Issuer = "InvenTrack",
        Audience = "InvenTrackClients",
        ExpiryMinutes = 60
    };

    private static JwtTokenGenerator CreateGenerator() =>
        new(Options.Create(Settings));

    [Fact]
    public void GenerateToken_ShouldReturnToken_AndFutureExpiration()
    {
        var before = DateTime.UtcNow;
        var generator = CreateGenerator();
        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = "John Doe",
            Email = "john@example.com"
        };

        var (token, expiration) = generator.GenerateToken(user, "Admin");

        token.Should().NotBeNullOrWhiteSpace();
        expiration.Should().BeAfter(before.AddMinutes(Settings.ExpiryMinutes - 1));
    }

    [Fact]
    public void GenerateToken_ShouldContainExpectedClaims()
    {
        var generator = CreateGenerator();
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            FullName = "John Doe",
            Email = "john@example.com"
        };

        var (token, _) = generator.GenerateToken(user, "Admin");

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId.ToString());
        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == "john@example.com");
        jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == "John Doe");
        jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
        jwt.Issuer.Should().Be(Settings.Issuer);
        jwt.Audiences.Should().Contain(Settings.Audience);
    }

    [Fact]
    public void GenerateToken_ShouldGenerateUniqueJti_PerCall()
    {
        var generator = CreateGenerator();
        var user = new User { Id = Guid.NewGuid(), FullName = "John Doe" };

        var (token1, _) = generator.GenerateToken(user, "Staff");
        var (token2, _) = generator.GenerateToken(user, "Staff");

        var handler = new JwtSecurityTokenHandler();
        var jti1 = handler.ReadJwtToken(token1).Claims
            .First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
        var jti2 = handler.ReadJwtToken(token2).Claims
            .First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;

        jti1.Should().NotBe(jti2);
    }

    [Fact]
    public void GenerateToken_ShouldHandleNullOrEmptyEmail()
    {
        var generator = CreateGenerator();
        var user = new User { Id = Guid.NewGuid(), FullName = "No Email", Email = null };

        var (token, _) = generator.GenerateToken(user, "Staff");

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == string.Empty);
    }
}
