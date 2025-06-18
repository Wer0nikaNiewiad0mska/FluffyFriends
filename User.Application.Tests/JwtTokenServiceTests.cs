using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using User.Application.Services;
using User.Application.Services.Interfaces;

namespace User.Application.Tests;

public class JwtTokenServiceTests
{
    private readonly Mock<IOptions<JwtSettings>> _mockSettings;
    private readonly JwtSettings _jwtSettings;
    private readonly JwtTokenService _jwtTokenService;
    private readonly string _testPrivateKeyPath;
    private readonly string _testPublicKeyPath;

    public JwtTokenServiceTests()
    {
        _mockSettings = new Mock<IOptions<JwtSettings>>();
        _jwtSettings = new JwtSettings
        {
            Issuer = "TestIssuer",
            Audience = "TestAudience"
        };
        _mockSettings.Setup(s => s.Value).Returns(_jwtSettings);

        _testPrivateKeyPath = "data/private.key";
        _testPublicKeyPath = "data/public.key";

        if (!Directory.Exists("data"))
        {
            Directory.CreateDirectory("data");
        }

        using (var rsa = RSA.Create(2048))
        {
            var privateKeyPem = rsa.ExportRSAPrivateKeyPem();
            File.WriteAllText(_testPrivateKeyPath, privateKeyPem);

            var publicKeyPem = rsa.ExportRSAPublicKeyPem();
            File.WriteAllText(_testPublicKeyPath, publicKeyPem);
        }

        _jwtTokenService = new JwtTokenService(_mockSettings.Object);
    }

    [Fact]
    public void GenerateToken_GeneratesValidJwt()
    {
        int userId = 123;
        string username = "testuser";
        List<string> roles = new List<string> { "Client", "Employee" };

        var token = _jwtTokenService.GenerateTOken(userId, username, roles);

        Assert.NotNull(token);
        Assert.NotEmpty(token);

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = GetPublicKey(),
            ClockSkew = TimeSpan.Zero 
        };

        ClaimsPrincipal principal;
        try
        {
            principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Token validation failed: {ex.Message}");
            return;
        }

        Assert.NotNull(principal);
        Assert.True(principal.HasClaim(ClaimTypes.NameIdentifier, userId.ToString()));
        Assert.True(principal.HasClaim(ClaimTypes.Name, username));
        Assert.Contains(principal.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Client");
        Assert.Contains(principal.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Employee");
        Assert.Equal(2, principal.Claims.Count(c => c.Type == ClaimTypes.Role));
    }

    [Fact]
    public void GenerateToken_WithNoRoles_GeneratesValidJwt()
    {
        int userId = 456;
        string username = "norolesuser";
        List<string> roles = new List<string>();

        var token = _jwtTokenService.GenerateTOken(userId, username, roles);

        Assert.NotNull(token);
        Assert.NotEmpty(token);

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = GetPublicKey(),
            ClockSkew = TimeSpan.Zero
        };

        ClaimsPrincipal principal;
        try
        {
            principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Token validation failed: {ex.Message}");
            return;
        }

        Assert.NotNull(principal);
        Assert.True(principal.HasClaim(ClaimTypes.NameIdentifier, userId.ToString()));
        Assert.True(principal.HasClaim(ClaimTypes.Name, username));

        Assert.DoesNotContain(principal.Claims, c => c.Type == ClaimTypes.Role);

    }

    private RsaSecurityKey GetPublicKey()
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(File.ReadAllText(_testPublicKeyPath));
        return new RsaSecurityKey(rsa);
    }

    public void Dispose()
    {
        if (File.Exists(_testPrivateKeyPath))
        {
            File.Delete(_testPrivateKeyPath);
        }
        if (File.Exists(_testPublicKeyPath))
        {
            File.Delete(_testPublicKeyPath);
        }
        if (Directory.Exists("data"))
        {
            
        }
    }
}
