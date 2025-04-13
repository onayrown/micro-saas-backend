using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.Services;
using MicroSaaS.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Moq;
using System; 
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using FluentAssertions;
using Xunit;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;

namespace MicroSaaS.Tests.Unit;

public class TokenServiceTests
{
    private readonly Mock<IOptions<JwtSettings>> _jwtSettingsMock;
    private readonly JwtSettings _jwtSettings;
    private readonly TokenService _tokenService;

    public TokenServiceTests()
    {
        _jwtSettings = new JwtSettings
        {
            SecretKey = Convert.ToBase64String(Encoding.UTF8.GetBytes("TestSecretKeySuperLongEnoughForHS256")),
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpirationInDays = 1 // Exemplo: 1 dia
        };
        _jwtSettingsMock = new Mock<IOptions<JwtSettings>>();
        _jwtSettingsMock.Setup(x => x.Value).Returns(_jwtSettings);
        _tokenService = new TokenService(_jwtSettingsMock.Object);
    }

    [Fact]
    public void GenerateToken_ShouldReturnValidJwtToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Username = "testuser", Role = "user" };

        // Act
        var token = _tokenService.GenerateToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
        jsonToken.Should().NotBeNull();
        jsonToken!.Issuer.Should().Be(_jwtSettings.Issuer);
        jsonToken.Audiences.Should().Contain(_jwtSettings.Audience);
        jsonToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId.ToString());
        jsonToken.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == userId.ToString());
        jsonToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == user.Email);
        jsonToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == user.Username);
        jsonToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == user.Role);
    }

    [Fact]
    public void ValidateToken_WithValidToken_ShouldReturnClaimsPrincipal()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Username = "testuser", Role = "user" };
        var token = _tokenService.GenerateToken(user);
        
        // Act
        var principal = _tokenService.ValidateToken(token); 

        // Assert
        principal.Should().NotBeNull();
        principal!.Identity!.IsAuthenticated.Should().BeTrue(); 
        principal.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == userId.ToString());
    }

    [Fact]
    public void ValidateToken_WithInvalidTokenFormat_ShouldReturnNull()
    {
        // Arrange
        var invalidToken = "invalid.token.string";

        // Act
        var principal = _tokenService.ValidateToken(invalidToken);

        // Assert
        principal.Should().BeNull(); 
    }
    
    [Fact]
    public void ValidateToken_WithExpiredToken_ShouldNotValidate()
    {
        // Renomeando o teste para refletir o comportamento real
        // Arrange - usar um token que está realmente expirado
        var expiredSettings = new JwtSettings
        {
            SecretKey = _jwtSettings.SecretKey,
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            ExpirationInDays = -1 // Token expirado
        };
        var expiredOptionsMock = new Mock<IOptions<JwtSettings>>();
        expiredOptionsMock.Setup(x => x.Value).Returns(expiredSettings);
        var expiredTokenService = new TokenService(expiredOptionsMock.Object);
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Username = "testuser", Role = "user" };
        var expiredToken = expiredTokenService.GenerateToken(user);

        // Act
        var principal = _tokenService.ValidateToken(expiredToken);

        // Assert
        principal.Should().BeNull("O token expirado deve ser considerado inválido");
    }

    [Fact]
    public void GetUserIdFromToken_WithValidToken_ShouldReturnUserIdString()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Username = "testuser", Role = "user" };
        var token = _tokenService.GenerateToken(user);

        // Act
        var extractedUserId = _tokenService.GetUserIdFromToken(token);

        // Assert
        extractedUserId.Should().Be(userId.ToString());
    }

    [Fact]
    public void GetUserIdFromToken_WithInvalidToken_ShouldReturnNull()
    { 
        // Arrange
        var invalidToken = "invalid.token";

        // Act
        var extractedUserId = _tokenService.GetUserIdFromToken(invalidToken);

        // Assert
        extractedUserId.Should().BeNull();
    }
} 