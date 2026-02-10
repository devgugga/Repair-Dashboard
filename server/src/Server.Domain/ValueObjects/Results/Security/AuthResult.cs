using Server.Domain.Entities.Core;

namespace Server.Domain.ValueObjects.Results.Security;

public class AuthResult
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime AccessTokenExpiresAt { get; set; }
    public DateTime RefreshTokenExpiresAt { get; set; }

    // User info
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public Guid PersonId { get; set; }
    public DateTimeOffset LastLogin { get; set; }

    public static AuthResult Success(
        string accessToken,
        string refreshToken,
        DateTime accessExpiry,
        DateTime refreshExpiry,
        User user)
    {
        return new AuthResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = accessExpiry,
            RefreshTokenExpiresAt = refreshExpiry,
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Person.Email,
            Role = user.Role.ToString(),
            PersonId = user.PersonId,
            LastLogin = user.LastLogin ?? DateTimeOffset.UtcNow
        };
    }
}
