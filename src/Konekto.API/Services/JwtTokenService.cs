using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Konekto.Domain.Entities;

namespace Konekto.API.Services;

public class JwtTokenService
{
    private readonly IConfiguration _configuration;
    public const string DefaultSecretKey = "Konekto_Secure_Alumni_SuperSecret_Jwt_SigningKey_2026_!#%&";
    public const string DefaultIssuer = "Konekto.API";
    public const string DefaultAudience = "Konekto.Clients";

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        var secretKey = _configuration["Jwt:SecretKey"] ?? DefaultSecretKey;
        var issuer = _configuration["Jwt:Issuer"] ?? DefaultIssuer;
        var audience = _configuration["Jwt:Audience"] ?? DefaultAudience;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role),
            new("role", user.Role),
            new("userId", user.Id.ToString()),
            new("fullName", $"{user.FirstName} {user.LastName}")
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        var secretKey = _configuration["Jwt:SecretKey"] ?? DefaultSecretKey;
        var issuer = _configuration["Jwt:Issuer"] ?? DefaultIssuer;
        var audience = _configuration["Jwt:Audience"] ?? DefaultAudience;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secretKey);

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5)
            }, out _);

            return principal;
        }
        catch
        {
            return null;
        }
    }
}
