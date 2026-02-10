using System.ComponentModel.DataAnnotations.Schema;

using Server.Domain.Entities.Core;

namespace Server.Domain.Entities.Security;

[Table("RefreshTokens")]
public class RefreshToken : BaseEntity
{
    public required Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;

    public required string Token { get; set; }

    public required DateTime ExpiresAt { get; set; }

    public bool IsRevoked { get; set; }

    public DateTime? RevokedAt { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public new bool IsActive => !IsRevoked && !IsExpired;

    /// <summary>
    ///     Revoga o refresh token
    /// </summary>
    public void Revoke()
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
