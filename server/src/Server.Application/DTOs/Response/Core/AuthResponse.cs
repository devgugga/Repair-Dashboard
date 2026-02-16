namespace Server.Application.DTOs.Response.Core;

/// <summary>
///     Authentication response payload returned by public endpoints.
/// </summary>
public class AuthResponse
{
    /// <summary>
    ///     Gets or sets the JWT access token.
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the access token expiration timestamp (UTC).
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    ///     Gets or sets the token type.
    /// </summary>
    public string TokenType { get; set; } = "Bearer";

    /// <summary>
    ///     Gets or sets authenticated user information.
    /// </summary>
    public AuthUserResponse User { get; set; } = new();
}

/// <summary>
///     Public user information included in authentication responses.
/// </summary>
public class AuthUserResponse
{
    /// <summary>
    ///     Gets or sets the user identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Gets or sets the username.
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the e-mail address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the user's primary role label.
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the latest successful login timestamp (UTC).
    /// </summary>
    public DateTime LastLogin { get; set; }
}
