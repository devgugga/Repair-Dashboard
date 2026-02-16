using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Server.Domain.Entities.Core;
using Server.Domain.Interfaces.Services.Core;
using Server.Domain.Interfaces.Services.Security;
using Server.Domain.ValueObjects.Options.Security;

namespace Server.Infrastructure.Services.Security;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _jwtOptions;
    private readonly IRbacService _rbacService;
    private readonly SymmetricSecurityKey _securityKey;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public JwtTokenService(IOptions<JwtOptions> jwtOptions, IRbacService rbacService)
    {
        _rbacService = rbacService;
        _jwtOptions = jwtOptions.Value;
        _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    // Backward compatibility - will be removed later
    public string GenerateAccessToken(User user)
    {
        return GenerateAccessTokenAsync(user).GetAwaiter().GetResult();
    }

    public async Task<string> GenerateAccessTokenAsync(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Person.Email),
            new("person_id", user.PersonId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };

        // Add roles to claims
        IEnumerable<string> roleNames = await _rbacService.GetUserRoleNamesAsync(user.Id);
        claims.AddRange(roleNames.Select(roleName => new Claim(ClaimTypes.Role, roleName)));

        // Add permissions to claims
        IEnumerable<string> permissions = await _rbacService.GetUserPermissionNamesAsync(user.Id);
        claims.AddRange(permissions.Select(permission => new Claim("permission", permission)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes),
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            SigningCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256)
        };

        SecurityToken? token = _tokenHandler.CreateToken(tokenDescriptor);
        return _tokenHandler.WriteToken(token);
    }


    public string GenerateRefreshToken()
    {
        byte[] randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public ClaimsPrincipal? GetClaimsFromToken(string token)
    {
        try
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = _jwtOptions.ValidateIssuer,
                ValidateAudience = _jwtOptions.ValidateAudience,
                ValidateLifetime = false, // Don't validate expiration here
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidAudience = _jwtOptions.Audience,
                IssuerSigningKey = _securityKey,
                ClockSkew = TimeSpan.FromMinutes(_jwtOptions.ClockSkewMinutes)
            };

            ClaimsPrincipal? principal = _tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    public bool ValidateAccessToken(string token)
    {
        try
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = _jwtOptions.ValidateIssuer,
                ValidateAudience = _jwtOptions.ValidateAudience,
                ValidateLifetime = _jwtOptions.ValidateLifetime,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidAudience = _jwtOptions.Audience,
                IssuerSigningKey = _securityKey,
                ClockSkew = TimeSpan.FromMinutes(_jwtOptions.ClockSkewMinutes)
            };

            _tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public Guid? GetUserIdFromToken(string token)
    {
        ClaimsPrincipal? claims = GetClaimsFromToken(token);
        string? userIdClaim = claims?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(userIdClaim, out Guid userId) ? userId : null;
    }

    public DateTime GetTokenExpiry(string token)
    {
        try
        {
            JwtSecurityToken? jwtToken = _tokenHandler.ReadJwtToken(token);
            return jwtToken.ValidTo;
        }
        catch
        {
            return DateTime.UtcNow; // Return current time if you can't parse
        }
    }
}
