namespace Server.Domain.ValueObjects.Options.Security;

public class JwtOptions
{
    public const string SectionName = "Security:Jwt";

    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required string Key { get; set; }

    /// <summary>
    ///     Access token expiration in minutes (default: 15)
    /// </summary>
    public int AccessTokenExpirationMinutes { get; set; } = 15;

    /// <summary>
    ///     Refresh token expiration in days (default: 15)
    /// </summary>
    public int RefreshTokenExpirationDays { get; set; } = 15;

    /// <summary>
    ///     Cookie name for refresh token
    /// </summary>
    public string RefreshTokenCookieName { get; set; } = "server_refresh";

    /// <summary>
    ///     Validate issuer
    /// </summary>
    public bool ValidateIssuer { get; set; } = true;

    /// <summary>
    ///     Validate audience
    /// </summary>
    public bool ValidateAudience { get; set; } = true;

    /// <summary>
    ///     Validate lifetime
    /// </summary>
    public bool ValidateLifetime { get; set; } = true;

    /// <summary>
    ///     Clock skew in minutes (default: 5)
    /// </summary>
    public int ClockSkewMinutes { get; set; } = 5;
}
